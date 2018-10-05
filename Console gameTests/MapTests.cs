using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Numerics;

namespace Console_game.Tests
{
    [TestClass()]
    public class MapTests
    {
        [TestMethod()]
        public void MapTestEmptyConstructorSucces()
        {
            // This one takes a while since the default constructor size is 600 x 150
            Map gameMap = new Map();

            // Assuring that the map has been filled
            Random randomGen = new Random();
            for (int i = 0; i < 50; i++)
            {
                float randomPosition1 = gameMap.map[
                    randomGen.Next(0, gameMap.map.GetLength(0)),
                    randomGen.Next(0, gameMap.map.GetLength(1))];
                Assert.AreEqual(true, randomPosition1 < 1 && randomPosition1 > -1);
            }


            // Variables being set
            Assert.AreEqual(true, gameMap.Seed != 0);
            Assert.AreEqual(true, gameMap.MapSizeX != 0);
            Assert.AreEqual(true, gameMap.MapSizeY != 0);
        }

        [TestMethod()]
        public void MapTestSeedConstructorSucces()
        {
            Map gameMap = new Map(new Random().Next(1, int.MaxValue));

            // Assuring that the map has been filled with proper values
            Random randomGen = new Random();
            for (int i = 0; i < 50; i++)
            {
                float randomPosition1 = gameMap.map[
                    randomGen.Next(0, gameMap.map.GetLength(0)),
                    randomGen.Next(0, gameMap.map.GetLength(1))];
                Assert.AreEqual(true, randomPosition1 < 1 && randomPosition1 > -1);
            }

            // Variables being set
            Assert.AreEqual(true, gameMap.Seed != 0);
            Assert.AreEqual(true, gameMap.MapSizeX != 0);
            Assert.AreEqual(true, gameMap.MapSizeY != 0);
        }

        [TestMethod()]
        [ExpectedException(typeof(ArgumentException))]
        public void MapTestSeedConstructorExceptionNegativeX()
        {
            Random randomGen = new Random();
            Map gameMap = new Map(randomGen.Next(), -20, randomGen.Next(1, 250), 1);
        }

        [TestMethod()]
        [ExpectedException(typeof(ArgumentException))]
        public void MapTestSeedConstructorExceptionNegativeY()
        {
            Random randomGen = new Random();
            Map gameMap = new Map(randomGen.Next(), randomGen.Next(1, 250), -20, 1);
        }

        [TestMethod()]
        public void GetPrintableMapTest()
        {
            Random randomGen = new Random();
            Map gameMap = new Map(randomGen.Next(), 30, 30, 1);
            ConsoleColor[,] mapColors = gameMap.GetPrintableMap(new Vector2(15, 15));
            Rectangle mapCutOut = gameMap.GetSeenMap(new Vector2(15, 15));

            for (int x = 0; x < mapCutOut.Width; ++x)
                for (int y = 0; y < mapCutOut.Height; ++y)
                {
                    float mapValue = gameMap.map[x + mapCutOut.X, y + mapCutOut.Y];

                    ConsoleColor tileColor;
                    if (mapValue < -0.75)
                    {
                        tileColor = ConsoleColor.Blue;
                    }
                    else if (mapValue < -0.5)
                    {
                        tileColor = ConsoleColor.Yellow;
                    }
                    else if (mapValue < -0.25)
                    {
                        tileColor = ConsoleColor.Green;
                    }
                    else if (mapValue < 0)
                    {
                        tileColor = ConsoleColor.Gray;
                    }
                    else if (mapValue < 0.25)
                    {
                        tileColor = ConsoleColor.White;
                    }
                    else
                    {
                        tileColor = ConsoleColor.Blue;
                    }

                    Assert.AreEqual(tileColor, mapColors[x, y]);
                }


        }

        // Getseenmaptests beyond this point
        Map testMap = new Map(30, 30);

        [TestMethod()]
        public void GetSeenMapNormalUsage()
        {
            Vector2 position = new Vector2(15, 15);

            Rectangle seenMap = testMap.GetSeenMap(position);
            Assert.AreEqual(position.X - testMap.PlayerViewRangeX, seenMap.X);
            Assert.AreEqual(position.Y - testMap.PlayerViewRangeY, seenMap.Y);

            Assert.AreEqual(testMap.PlayerViewRangeX * 2, seenMap.Width);
            Assert.AreEqual(testMap.PlayerViewRangeY * 2, seenMap.Height);
        }

        [TestMethod()]
        public void GetSeenMapRightEdge()
        {
            Vector2 position = new Vector2(29, 15);

            Rectangle seenMap = testMap.GetSeenMap(position);
            Assert.AreEqual(position.X - testMap.PlayerViewRangeX, seenMap.X);
            Assert.AreEqual(position.Y - testMap.PlayerViewRangeY, seenMap.Y);

            Assert.AreEqual(testMap.PlayerViewRangeX * 2, seenMap.Width);
            Assert.AreEqual(testMap.PlayerViewRangeY * 2, seenMap.Height);
        }

        [TestMethod()]
        public void GetSeenMapLeftEdge()
        {
            Vector2 position = new Vector2(0, 15);

            Rectangle seenMap = testMap.GetSeenMap(position);
            Assert.AreEqual(0, seenMap.X);
            Assert.AreEqual(position.Y - testMap.PlayerViewRangeY, seenMap.Y);

            Assert.AreEqual(testMap.PlayerViewRangeX * 2, seenMap.Width);
            Assert.AreEqual(testMap.PlayerViewRangeY * 2, seenMap.Height);
        }

        [TestMethod()]
        public void GetSeenMapTop()
        {
            Vector2 position = new Vector2(15, 0);

            Rectangle seenMap = testMap.GetSeenMap(position);
            Assert.AreEqual(position.X - testMap.PlayerViewRangeX, seenMap.X);
            Assert.AreEqual(0, seenMap.Y);

            Assert.AreEqual(testMap.PlayerViewRangeX * 2, seenMap.Width);
            Assert.AreEqual(testMap.PlayerViewRangeY * 2, seenMap.Height);
        }

        [TestMethod()]
        public void GetSeenMapBottom()
        {
            Vector2 position = new Vector2(15, 29);

            Rectangle seenMap = testMap.GetSeenMap(position);
            Assert.AreEqual(position.X - testMap.PlayerViewRangeX, seenMap.X);
            Assert.AreEqual(position.Y - testMap.PlayerViewRangeY, seenMap.Y);

            Assert.AreEqual(testMap.PlayerViewRangeX * 2, seenMap.Width);
            Assert.AreEqual(testMap.PlayerViewRangeY * 2, seenMap.Height);
        }

        [TestMethod()]
        public void GetSeenMapOutOfBoundsRight()
        {
            Vector2 position = new Vector2(60, 15);

            Rectangle seenMap = testMap.GetSeenMap(position);
            Assert.AreEqual(position.X - testMap.PlayerViewRangeX, seenMap.X);
            Assert.AreEqual(position.Y - testMap.PlayerViewRangeY, seenMap.Y);

            Assert.AreEqual(testMap.PlayerViewRangeX * 2, seenMap.Width);
            Assert.AreEqual(testMap.PlayerViewRangeY * 2, seenMap.Height);
        }

        [TestMethod()]
        public void GetSeenMapOutOfBoundsLeft()
        {
            Vector2 position = new Vector2(-20, 15);

            Rectangle seenMap = testMap.GetSeenMap(position);
            Assert.AreEqual(0, seenMap.X);
            Assert.AreEqual(position.Y - testMap.PlayerViewRangeY, seenMap.Y);

            Assert.AreEqual(testMap.PlayerViewRangeX * 2, seenMap.Width);
            Assert.AreEqual(testMap.PlayerViewRangeY * 2, seenMap.Height);
        }

        [TestMethod()]
        public void GetSeenMapOutOfBoundsTop()
        {
            Vector2 position = new Vector2(15, -20);

            Rectangle seenMap = testMap.GetSeenMap(position);
            Assert.AreEqual(position.X - testMap.PlayerViewRangeX, seenMap.X);
            Assert.AreEqual(0, seenMap.Y);

            Assert.AreEqual(testMap.PlayerViewRangeX * 2, seenMap.Width);
            Assert.AreEqual(testMap.PlayerViewRangeY * 2, seenMap.Height);
        }

        [TestMethod()]
        public void GetSeenMapOutOfBoundsBottom()
        {
            Vector2 position = new Vector2(15, 60);

            Rectangle seenMap = testMap.GetSeenMap(position);
            Assert.AreEqual(position.X - testMap.PlayerViewRangeX, seenMap.X);
            Assert.AreEqual(position.Y - testMap.PlayerViewRangeY, seenMap.Y);

            Assert.AreEqual(testMap.PlayerViewRangeX * 2, seenMap.Width);
            Assert.AreEqual(testMap.PlayerViewRangeY * 2, seenMap.Height);
        }
    }
}