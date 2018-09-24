using System;

namespace Console_game
{
    class Program
    {
        static DateTime start = DateTime.Now;
        static float total;

        static void print1(object state)
        {
            
            //Console.Clear();
            total += FrameInfo.timeDelta;
            //Console.Write($"{(DateTime.Now - start).Seconds},             {(int)total}");
        }

        static void Main(string[] args)
        {

            Globals.logger.logInfo("DAB");
            Console.Write("hai\n");

            Console.ReadKey();
            for (int i = 0; i < 100; i++)
            {
                Win32Console.SetConsoleFont();
                Console.ReadKey();
            }
            Console.Write("hai\n");
            Console.ReadKey();


            FrameRunner frameRunner = new FrameRunner();
            frameRunner += print1;

            Console.ReadKey();
            frameRunner.Pause();
            Console.ReadKey();





            Console.SetWindowSize(120, 30);
            Console.SetBufferSize(120, 30);
            Map thisMap = new Map();

            printMap(thisMap);
            Console.ReadKey();
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
