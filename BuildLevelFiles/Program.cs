﻿using System;
using System.Diagnostics;
using System.IO;
using System.Text;

namespace BuildLevelFiles
{
    class Program
    {
        static void Main(string[] args)
        {
            CheckDir();
            Console.WriteLine("Введите имя уровня");
            string Level = "", LevelName = Console.ReadLine().Trim(' ');
            DateTime StartTime;
            Level = LevelName;
            Console.Clear();
            Console.WriteLine("Выбрано: " + Level);
            CorrectingGamedataFiles(LevelName);
            Console.WriteLine("Выберите задачи:" + Environment.NewLine + "Введите: 0 - чтобы выполнить все задачи без упаковки архива в xdb" + Environment.NewLine + "Введите: 1 - чтобы собрать только архив для: " + Level+Environment.NewLine + "Введите: 2 - чтобы выполнить все задачи с упаковкой карты в *.xdb*" +Environment.NewLine + "Введите: 3 - чтобы собрать архив только для: " + Level + ", с упаковкой карты в *.xdb*");
            int type = Convert.ToInt32(Console.ReadLine());
            if (type == 1 || type == 3)
            {           
                switch (type)
                {
                    case 1:
                        CopyGamedataFiles(LevelName);
                        break;
                    case 3:
                        CopyGamedataFiles(LevelName);
                        Console.WriteLine("[" + DateTime.Now + "] Упаковка в xdb...");
                        Start(@"SdkSoft\converter_xdb\converter.exe", "-pack "+Environment.CurrentDirectory + "\\BuildLevelFiles\\"+ Level + "\\gamedata" + " -out BuildLevelFiles\\" + Level + "\\" + Level + ".xdb0");
                        break;
                }
                if (Directory.Exists("BuildLevelFiles\\" + Level + "\\"))
                    Process.Start("BuildLevelFiles\\" + Level + "\\");
                Console.WriteLine("[" + DateTime.Now + "] Готово");
                Console.ReadKey();
                return;
            }
            Console.WriteLine("Выполнить компиляцию карты? Параметр Compiler.exe -f" + Environment.NewLine + "Введите: 1 - для компиляции карты" + Environment.NewLine + "Введите: 0 - для пропуска этого действия");
            if (Convert.ToInt32(Console.ReadLine()) == 1)
            {
                StartTime = DateTime.Now.AddSeconds(10);
                Console.WriteLine("[" + DateTime.Now + "] Сборка карты: " + LevelName);
                Start("SdkSoft\\Compiler.exe", "-f " + LevelName);
                if (StartTime.TimeOfDay > DateTime.Now.TimeOfDay)
                {
                    Console.WriteLine("Возникла ошибка компиляции. Процесс 'Compiler.exe' принудительно завершил свою работу." + Environment.NewLine + "Продолжить выполнение?" + Environment.NewLine + "0 - нет" + Environment.NewLine + "1 - да");
                    switch (Console.ReadLine())
                    {
                        case "0":
                            return;
                    }
                }
                Console.WriteLine("[" + DateTime.Now + "] Сборка карты завершена");
            }
            string[,] Key_Level = new string[2, 4] { { "-f ", "-g ", "-m", "-no_separator_check -noverbose -s" }, { "1", "1", "0", "0" } };
            for (int i = 0; 4 > i; i++)
            {
                if (Key_Level[1, i] == "0")
                    LevelName = null;
                Console.WriteLine("[" + DateTime.Now + "] start arg: " + Key_Level[0, i] + " " + LevelName);
                Start(@"SdkSoft\\compiler_ai\\xrai.exe", Key_Level[0, i] + LevelName);
                Console.WriteLine("[" + DateTime.Now + "] completed: " + Key_Level[0, i] + " " + LevelName);
            }
            CopyGamedataFiles(Level);
            if (Directory.Exists("BuildLevelFiles\\" + Level + "\\")) 
                Process.Start("BuildLevelFiles\\" + Level + "\\");
            if (type == 2)
            {
                Console.WriteLine("[" + DateTime.Now + "] Упаковка в xdb...");
                Start(@"SdkSoft\converter_xdb\converter.exe", "-pack " + Environment.CurrentDirectory + "\\BuildLevelFiles\\" + Level + "\\gamedata" + " -out BuildLevelFiles\\" + Level + "\\" + Level + ".xdb0");
            }
            Console.WriteLine("[" + DateTime.Now + "] Готово");
            Console.ReadKey();
        }

        static void CheckDir()
        {
            if (!Directory.Exists(Environment.CurrentDirectory + "\\BuildLevelFiles"))
                Directory.CreateDirectory(Environment.CurrentDirectory + "\\BuildLevelFiles");
        }

        static void CorrectingGamedataFiles(string LevelName)
        {
            if (Directory.Exists("gamedata\\config"))
            {
                string FileInfo =
                    "[levels]" + Environment.NewLine +
                    "level01" + Environment.NewLine + Environment.NewLine +
                    "[level01]" + Environment.NewLine +
                    "name = " + LevelName + Environment.NewLine +
                    "caption = \"" + LevelName + "\"" + Environment.NewLine +
                    "offset  = 3000.0,1000.0,0.0" + Environment.NewLine +
                    "id = 01";
                File.WriteAllText("gamedata\\config\\game_levels.ltx", FileInfo, Encoding.GetEncoding(1251));
                Console.WriteLine("Данные в game_levels.ltx успешно записаны");
            }
        }

        static void CopyGamedataFiles(string Level)
        {
            Console.WriteLine("[" + DateTime.Now + "] Создаем новую директорию... BuildLevelFiles\\gamedata\\" + Level);
            foreach (string _Directory in Directory.GetDirectories("gamedata\\config", "*", SearchOption.AllDirectories))
            {
                Directory.CreateDirectory(_Directory.Replace("gamedata\\config", "BuildLevelFiles\\" + Level + "\\gamedata\\config"));
            }
            Console.WriteLine("[" + DateTime.Now + "] Копируем файлы в новую директорию...");
            foreach (string _Files in Directory.GetFiles("gamedata\\config", "*.*", SearchOption.AllDirectories))
            {
                File.Copy(_Files, _Files.Replace("gamedata\\config", "BuildLevelFiles\\" + Level + "\\gamedata\\config"), true);
            }
            Console.WriteLine("[" + DateTime.Now + "] Копируем в BuildLevelFiles\\" + Level + "\\gamedata\\levels\\" + Level);
            if (!Directory.Exists("BuildLevelFiles\\" + Level + "\\gamedata\\levels\\" + Level + "\\"))
                Directory.CreateDirectory("BuildLevelFiles\\" + Level + "\\gamedata\\levels\\" + Level);
            foreach (string CreateAllDir in Directory.GetDirectories("gamedata\\levels\\" + Level + "\\", "*", SearchOption.AllDirectories))
            {
                Directory.CreateDirectory(CreateAllDir.Replace("gamedata\\levels\\" + Level + "\\", "BuildLevelFiles\\" + Level + "\\gamedata\\levels\\" + Level + "\\"));
            }
            foreach (string _Files in Directory.GetFiles("gamedata\\levels\\" + Level, "*.*", SearchOption.AllDirectories))
            {
                File.Copy(_Files, _Files.Replace("gamedata\\levels\\" + Level, "BuildLevelFiles\\" + Level + "\\gamedata\\levels\\" + Level), true);
            }
            Console.WriteLine("[" + DateTime.Now + "] Копируем: " + Level + ".spawn");
            if (!Directory.Exists(Environment.CurrentDirectory + "\\BuildLevelFiles\\" + Level + "\\gamedata\\spawns"))
                Directory.CreateDirectory(Environment.CurrentDirectory + "\\BuildLevelFiles\\" + Level + "\\gamedata\\spawns");
            if (File.Exists("gamedata\\spawns\\" + Level + ".spawn"))
                File.Copy("gamedata\\spawns\\" + Level + ".spawn", "BuildLevelFiles\\" + Level + "\\gamedata\\spawns\\" + Level + ".spawn", true);
            else
                Console.WriteLine("NOT FOUND -> gamedata\\" + Level + "\\spawns\\" + Level + ".spawn");
            Console.WriteLine("[" + DateTime.Now + "] Копируем game.graph");
            if (File.Exists("gamedata\\game.graph"))
                File.Copy("gamedata\\game.graph", "BuildLevelFiles\\" + Level + "\\gamedata\\game.graph", true);
            else
                Console.WriteLine("NOT FOUND -> gamedata\\game.graph");
            Console.WriteLine("[" + DateTime.Now + "] Удаляем ненужные файлы в BuildLevelFiles\\" + Level + "\\gamedata\\levels\\" + Level + "\\build.*");
            foreach (string _Files in Directory.GetFiles("BuildLevelFiles\\" + Level + "\\gamedata\\levels\\" + Level, "build.*", SearchOption.AllDirectories))
            {
                File.Delete(_Files);
                Console.WriteLine("deleted -> " + _Files);
            }
        }

        static void Start(string Exe, string Args)
        {
            Process Compile = new Process();
            Compile.StartInfo.FileName = Exe;
            Compile.StartInfo.Arguments = Args;
            Compile.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
            Compile.Start();
            Compile.WaitForExit();
            Compile.Close();
        }
    }
}