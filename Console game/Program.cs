using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Windows.Input;
using System.IO;
using static Console_game.NativeMethods;

namespace Console_game
{
    class Program
    {
        private static float total;
        private static void DisplayTimeAccuracy()
        {
            total += GameObject.timeDelta;
            Console.Clear();
            Console.Write($"time: {GameObject.time}\ntimedelta total: {total}\ntimedelta: {GameObject.timeDelta}\ndifference: {total -GameObject.time}");
        }

        public static void print2()
        {
            if (Input.GetKeyDown('a'))
            {
                Console.Write("down\n");
            }
            if (Input.GetKeyHeld('a'))
            {
                Console.Write("holding\n");
            }
            if (Input.GetKeyUp('a'))
            {
                Console.Write("up\n");
            }
        }

        static void Main(string[] args)
        {
            // Set the console's title to a preset gamename
            Win32ConsoleHelper.SetConsoleTitle(Globals.gameName);

            ConsoleListener.MouseEvent += InternalInput.MouseSetter;
            ConsoleListener.KeyEvent += InternalInput.KeySetter;

            Globals.GameMethodSignature frameSubscribers = new ReflectiveHelper<GameObject>().GetMethodsByString("update");
            frameSubscribers += DisplayTimeAccuracy;
            ConsoleListener.Start();

			FrameRunner.AddFrameSubscriber(frameSubscribers);
			FrameRunner.Start();

            // Creating the necessary folders and files
            Directory.CreateDirectory("logs");
            using (StreamWriter x = File.AppendText("logs/log.txt"))


            Console.SetBufferSize(120, 30);
            Console.SetWindowSize(120, 30);
            Map thisMap = new Map();

            printMap(thisMap);

            Console.ReadKey(false);
        }

        private const string blockChar = "█";
        static void printMap(Map map)
        {
            ConsoleColor[,] mapColors = map.getPrintableMap();
            for (int y = 0; y < mapColors.GetLength(0); y++)
            {
                for (int x = 0; x < mapColors.GetLength(1); x++)
                {
                    Console.ForegroundColor = mapColors[y, x];
                    Console.Write(blockChar);
                }
            }
        }
    }
}
