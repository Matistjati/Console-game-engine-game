using System;
using System.Collections.Generic;

namespace Console_game
{
    class Program
    {
        static readonly DateTime start = DateTime.Now;
        static float total;

        public static void print1()
        {
            Console.Write(1);
            /*
            Console.Clear();
            total += GameObject.timeDelta;
            Console.Write($"{(DateTime.Now - start).Seconds},{(DateTime.Now - start).Milliseconds} {total}");*/
        }

        public static void print2()
        {
            Console.Write(2);
            
        }

        public delegate void frameUpdateSubscriber();

        static void Main(string[] args)
        {
            //GameObjectChildren<GameObject>.frameCallSubscribers  maaboi = new GameObjectChildren<GameObject>().GetMethodsByString("update");
            //maaboi.Invoke();
            frameUpdateSubscriber frameBoi = new frameUpdateSubscriber(print1);

            Console.ReadKey();
            //frameRunner.Pause();
            Console.ReadKey();
            //frameRunner.Run();
            Console.ReadKey();






            Console.SetBufferSize(120, 30);
            Console.SetWindowSize(120, 30);
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
