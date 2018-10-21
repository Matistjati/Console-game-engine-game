using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Reflection;

namespace Console_game
{
    internal static class FrameRunner
    {
        static DateTime lastFrameCall;
        static DateTime start;

        static bool run;

        static bool firstRun = true;

        static DateTime pauseStartTime;
        public static void Pause()
        {
            run = false;
            pauseStartTime = DateTime.Now;
        }

        private static string[,] displayArea;
        static Coord displaySize;

        public static void Run()
        {
            if (firstRun)
            {
                start = DateTime.Now;
                firstRun = false;
            }
            else
            {
                start += DateTime.Now - pauseStartTime;
            }

            lastFrameCall = DateTime.Now;

            displayArea = new string[Console.WindowWidth, Console.WindowHeight];
            displaySize = new Coord(displayArea.GetLength(0), displayArea.GetLength(1));

            run = true;
            while (run)
            {
                // Calculating and setting timedelta
                GameObject._timeDelta = (float)(DateTime.Now - lastFrameCall).TotalSeconds;

                GameObject._time = (float)(DateTime.Now - start).TotalSeconds;

                Input.UpdateInput();

                lastFrameCall = DateTime.Now;

                // Calling update
                foreach (KeyValuePair<MethodInfo, Component> method in updateCallBack)
                {
                    method.Value.Invoke(method.Key);
                }


                // Rendering
                Console.Clear();
                for (int y = 0; y < displaySize.Y; y++)
                {
                    for (int x = 0; x < displaySize.X; x++)
                    {

                        displayArea[x, y] = " ";
                    }
                }

                for (int i = 0; i < renderedGameObjects.Count; i++)
                {
                    if (spriteDisplayers[i].IsVisible)
                    {
                        Coord position = (Coord)renderedGameObjects[i].physicalState.Position;
                        Coord consoleSize = new Coord(displayArea.GetLength(0), displayArea.GetLength(1));
                        Coord colorMapSize = new Coord(spriteDisplayers[i].ColorMap.GetLength(0), spriteDisplayers[i].ColorMap.GetLength(0));
                        if (position.X < consoleSize.X && position.Y < consoleSize.Y)
                        {
                            string displayChar = spriteDisplayers[i].PrintedChar;
                            for (int x = 0; x < colorMapSize.X; x++)
                            {
                                for (int y = 0; y < colorMapSize.Y; y++)
                                {
                                    if (x < consoleSize.X && y < consoleSize.Y && x + position.X < displaySize.X && y + position.Y < displaySize.Y)
                                    {
                                        Color color = spriteDisplayers[i].ColorMap[x, y];
                                        displayArea[x + position.X, y + position.Y] = $"\x1b[38;2;{color.R};{color.G};{color.B}m" + displayChar;
                                    }
                                }
                            }
                        }

                        for (uint y = position.Y; y < position.Y + colorMapSize.Y; y++)
                        {
                            if (y < displaySize.Y)
                            {
                                displayArea[displaySize.X - 1, y] += Environment.NewLine;
                            }
                        }
                    }
                }

                for (int y = 0; y < displaySize.Y; y++)
                {
                    for (int x = 0; x < displaySize.X; x++)
                    {
                        Console.Write(displayArea[x, y]);
                    }
                }

                //Thread.Sleep(100);
            }
        }


        static List<SpriteDisplayer> spriteDisplayers = new List<SpriteDisplayer>();
        static List<GameObject> renderedGameObjects;
        public static List<GameObject> RenderedGameObjects
        {
            internal get => renderedGameObjects ?? null;
            set
            {
                spriteDisplayers.AddRange(value.Select(x => x.GetComponent<SpriteDisplayer>()));
                renderedGameObjects = value;
            }
        }


        public static Dictionary<MethodInfo, Component> updateCallBack = new Dictionary<MethodInfo, Component>();

        internal static void AddFrameSubscriber(KeyValuePair<MethodInfo, Component> method)
        {
            if (!updateCallBack.Keys.Contains(method.Key))
            {
                updateCallBack.Add(method.Key, method.Value);
            }
        }

        internal static void AddFrameSubscriber(Dictionary<MethodInfo, Component> method)
        {
            foreach (KeyValuePair<MethodInfo, Component> methodInfo in method)
            {
                if (!updateCallBack.Keys.Contains(methodInfo.Key))
                {
                    updateCallBack.Add(methodInfo.Key, methodInfo.Value);
                }
            }
        }

        internal static void AddFrameSubscriber(MethodInfo method, object instance)
        {
            if (!updateCallBack.Keys.Contains(method))
            {
                updateCallBack.Add(method, (Component)instance);
            }
        }

        internal static void Unsubscribe(MethodInfo method)
        {
            if (updateCallBack.Keys.Contains(method))
            {
                updateCallBack.Remove(method);
            }
        }

        public static void UnsubscribeAll()
        {
            updateCallBack.Clear();
        }
    }
}
