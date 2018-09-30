using System;
using System.Collections.Generic;
using System.Windows.Input;
using static Console_game.NativeMethods;

namespace Console_game
{
    class Program
    {
        static readonly DateTime start = DateTime.Now;
        static float total;

        public static void print1(KEY_EVENT_RECORD r)
        {
            //Console.Write(r.UnicodeChar);
            //Console.Write(r.UnicodeChar);
            /*
            Console.Clear();
            total += GameObject.timeDelta;
            Console.Write($"{(DateTime.Now - start).Seconds},{(DateTime.Now - start).Milliseconds} {total}");*/
        }

        public static void print2()
        {
            if (Input.keysDown['a'])
            {
                Console.Write("down\n");
            }
            if (Input.keysHeld['a'])
            {
                Console.Write("holding\n");
            }
            if (Input.keysUp['a'])
            {
                Console.Write("up\n");
            }
        }

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
            frameSubscribers += print2;
            ConsoleListener.Start();

            DateTime lastFrameCall = DateTime.Now;
            while (true)
            {
                // Calculating timedelta and incrementing time
                float timeDelta = (DateTime.Now - lastFrameCall).Ticks / 10000000;
                GameObject.timeDelta = timeDelta;
                GameObject.time += timeDelta;

                Input.UpdateInput();

                frameSubscribers.Invoke();


                lastFrameCall = DateTime.Now;
                
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
