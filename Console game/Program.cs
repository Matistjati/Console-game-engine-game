using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Windows.Input;
using System.IO;
using System.Numerics;
using static Console_game.NativeMethods;

namespace Console_game
{
    class Program
    {
        private static float total;
        private static void TestTimeAccuracy()
        {
            total += GameObject.timeDelta;
            Console.Clear();
            Console.Write($"time: {GameObject.time}\ntimedelta total: {total}\ntimedelta: {GameObject.timeDelta}\ndifference: {total -GameObject.time}");
        }

        private static void TestInputAccuracy()
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

        static void GameSetup()
        {
            // Set the console's title to a preset gamename
            Win32ConsoleHelper.SetConsoleTitle(Globals.gameName);
            Win32ConsoleHelper.SetConsoleFontSize(18, 18);

            // Set up input handlelers
            ConsoleListener.MouseEvent += InternalInput.MouseSetter;
            ConsoleListener.KeyEvent += InternalInput.KeySetter;
            ConsoleListener.Start();

            // Getting all classes deriving from gameobject and getting update and start methods
            ReflectiveHelper<GameObject> gameObjectChildren = new ReflectiveHelper<GameObject>();
            Globals.GameMethodSignature frameSubscribers = gameObjectChildren.GetMethodsByString("update");
            Globals.GameMethodSignature gameStartup = gameObjectChildren.GetMethodsByString("start");
            gameStartup.Invoke();
            frameSubscribers += TestTimeAccuracy;
            FrameRunner.AddFrameSubscriber(frameSubscribers);

            // Testing
            frameSubscribers += TestTimeAccuracy;

            // Creating the necessary folders and files
            Directory.CreateDirectory("logs");
            using (StreamWriter x = File.AppendText("logs/log.txt")) { }

            // Starting
            FrameRunner.Start();
        }

        static void Main(string[] args)
        {
            GameSetup();

            Console.SetBufferSize(120, 30);
            Console.SetWindowSize(120, 30);
            Map thisMap = new Map();

            printMap(thisMap);

            Console.ReadKey(false);
        }

        private const string blockChar = "█";
        static void printMap(Map map)
        {
            ConsoleColor[,] mapColors = map.getPrintableMap(new Vector2(15, 15));
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
