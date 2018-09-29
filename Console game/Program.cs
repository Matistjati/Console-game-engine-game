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

        public static void print2(MOUSE_EVENT_RECORD r)
        {
            if (r.dwButtonState == MOUSE_EVENT_RECORD.FROM_LEFT_1ST_BUTTON_PRESSED)
            {
                Console.Write(2);
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
            ConsoleListener.MouseEvent += print2;
            ConsoleListener.KeyEvent += print1;

            //ReflectiveHelper<GameObject>.methodSignature  maaboi = new ReflectiveHelper<GameObject>().GetMethodsByString("update");
            //maaboi.Invoke();
            ConsoleListener.Start();
            Console.ReadKey(false);
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
