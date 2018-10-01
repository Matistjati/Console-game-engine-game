using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Windows.Input;
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

        private static readonly TimeSpan frameWait = new TimeSpan(166667);

        static DateTime start;

        static void Main(string[] args)
        {
            // Set the console's title to a preset gamename
            Win32ConsoleHelper.SetConsoleTitle(Globals.gameName);

            IntPtr inHandle = GetStdHandle(STD_INPUT_HANDLE);
            uint mode = 0;
            GetConsoleMode(inHandle, ref mode);
            mode &= ~ENABLE_QUICK_EDIT_MODE; //disable
            mode |= ENABLE_WINDOW_INPUT; //enable (if you want)
            mode |= ENABLE_MOUSE_INPUT; //enable
            SetConsoleMode(inHandle, mode);

            ConsoleListener.MouseEvent += InternalInput.MouseSetter;
            ConsoleListener.KeyEvent += InternalInput.KeySetter;

            ReflectiveHelper<GameObject>.methodSignature frameSubscribers = new ReflectiveHelper<GameObject>().GetMethodsByString("update");
            frameSubscribers += DisplayTimeAccuracy;
            ConsoleListener.Start();

            DateTime lastFrameCall = DateTime.Now;
            start = DateTime.Now;
            while (true)
            {
                // Calculating and setting timedelta
                GameObject.timeDelta = (float)(DateTime.Now - lastFrameCall).TotalSeconds;

                GameObject.time = (float)(DateTime.Now - start).TotalSeconds;

                Input.UpdateInput();

                lastFrameCall = DateTime.Now;
                frameSubscribers.Invoke();


                System.Threading.Thread.Sleep(frameWait);
            }
            //frameRunner.Pause();
            Console.ReadKey(false);
            //frameRunner.Run();
            Console.ReadKey(false);






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
