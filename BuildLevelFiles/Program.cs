using System;
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
            Console.Clear();
            Console.WriteLine("Выбрано: " + LevelName);
            Console.WriteLine("Выполнить компиляцию карты? Параметр binaries_x64_release\\Compiler.exe -f" + Environment.NewLine + "Введите: 1 - для компиляции карты" + Environment.NewLine + "Введите: 0 - для пропуска этого действия");
            Level = LevelName;
            if (Convert.ToInt32(Console.ReadLine()) == 1)
            {
                Console.WriteLine("Сборка карты: " + LevelName);
                Start("binaries_x64_release\\Compiler.exe", "-f " + LevelName);
                Console.WriteLine("Компиляция карты завершена");
            }
            CorrectingGamedataFiles(LevelName);
            string[,] Key_Level = new string[2, 4] { { "-f ", "-g ", "-m", "-no_separator_check -noverbose -s" }, { "1", "1", "0", "0" } };
            for (int i = 0; 4 > i; i++)
            {
                if (Key_Level[1, i] == "0")
                    LevelName = null;

                Console.WriteLine("start arg: " + Key_Level[0, i] + " " + LevelName);
                Start(@"bins\compiler_x64\xrai.exe", Key_Level[0, i] + LevelName);
            }
            CopyGamedataFiles(Level);
            Console.WriteLine("Готово");
            Console.ReadKey();
        }

        static void CheckDir()
        {
            if (!Directory.Exists(Environment.CurrentDirectory + "\\BuildLevelFiles"))
                Directory.CreateDirectory(Environment.CurrentDirectory + "\\BuildLevelFiles");
            if (!Directory.Exists(Environment.CurrentDirectory+ "\\BuildLevelFiles\\gamedata\\spawns"))
                Directory.CreateDirectory(Environment.CurrentDirectory + "\\BuildLevelFiles\\gamedata\\spawns");
        }

        static void CorrectingGamedataFiles(string LevelName)
        {
            if (Directory.Exists("gamedata\\config"))
            {
                string FileInfo =
                    "[levels]" + Environment.NewLine +
                    "level01" + Environment.NewLine + Environment.NewLine +
                    "[level01]" + Environment.NewLine +
                    "name= " + LevelName + Environment.NewLine +
                    "caption = " + LevelName + Environment.NewLine +
                    "offset  = 3000.0,1000.0,0.0" + Environment.NewLine +
                    "id = 01";
                File.WriteAllText("gamedata\\config\\game_levels.ltx", FileInfo, Encoding.GetEncoding(1251));
                Console.WriteLine("CorrectingGamedataFiles - success");
            }
        }

        static void CopyGamedataFiles(string Level)
        {
            Console.WriteLine("Создаем новую директорию... BuildLevelFiles\\gamedata");
            foreach (string _Directory in Directory.GetDirectories("gamedata\\config", "*", SearchOption.AllDirectories))
            {
                Directory.CreateDirectory(_Directory.Replace("gamedata\\config", "BuildLevelFiles\\gamedata\\config"));
            }
            Console.WriteLine("Копируем файлы в новую директорию...");
            foreach (string _Files in Directory.GetFiles("gamedata\\config", "*.*", SearchOption.AllDirectories))
            {
                File.Copy(_Files, _Files.Replace("gamedata\\config", "BuildLevelFiles\\gamedata\\config"), true);
            }    
            Console.WriteLine("Копируем в BuildLevelFiles\\gamedata\\levels\\" + Level);            
            foreach (string CreateAllDir in Directory.GetDirectories("gamedata\\levels\\" + Level +"\\", "*", SearchOption.AllDirectories))
            {
                Directory.CreateDirectory(CreateAllDir.Replace("gamedata\\levels\\" + Level + "\\", "BuildLevelFiles\\gamedata\\levels\\" + Level + "\\"));
            }
            foreach (string _Files in Directory.GetFiles("gamedata\\levels\\" + Level, "*.*", SearchOption.AllDirectories))
            {
                File.Copy(_Files, _Files.Replace("gamedata\\levels\\"+ Level, "BuildLevelFiles\\gamedata\\levels\\" + Level), true);
            }          
            Console.WriteLine("Копируем: " + Level + ".spawn");
            if (File.Exists("gamedata\\spawns\\" + Level + ".spawn"))
                File.Copy("gamedata\\spawns\\" + Level + ".spawn", "BuildLevelFiles\\gamedata\\spawns\\" + Level + ".spawn", true);
            else
                Console.WriteLine("NOT FOUND -> gamedata\\spawns\\" + Level + ".spawn");    
            Console.WriteLine("Копируем game.graph");
            if (File.Exists("gamedata\\game.graph"))
                File.Copy("gamedata\\game.graph", "BuildLevelFiles\\gamedata\\game.graph", true);
            else
                Console.WriteLine("NOT FOUND -> gamedata\\game.graph");
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