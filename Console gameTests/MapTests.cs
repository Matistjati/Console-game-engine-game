using Microsoft.VisualStudio.TestTools.UnitTesting;
using Console_game;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace Console_game.Tests
{
    [TestClass()]
    public class MapTests
    {
        [TestMethod()]
        public void MapTestEmptyConstructorSucces()
        {
            Map gameMap = new Map();

            // Assuring that the map has been filled
            Random randomGen = new Random();
            Assert.AreEqual(true, gameMap.map[randomGen.Next(0, gameMap.map.GetLength(0)), randomGen.Next(0, gameMap.map.GetLength(1))] != 0 &&
                                  gameMap.map[randomGen.Next(0, gameMap.map.GetLength(0)), randomGen.Next(0, gameMap.map.GetLength(1))] != 0);

            // Variables being set
            Assert.AreEqual(true, gameMap.seed != 0);
            Assert.AreEqual(true, gameMap.mapSizeX != 0);
            Assert.AreEqual(true, gameMap.mapSizeY != 0);
        }

        [TestMethod()]
        public void MapTestSeedConstructorSucces()
        {
            Map gameMap = new Map(new Random().Next(1, int.MaxValue));

            // Assuring that the map has been filled
            Random randomGen = new Random();
            Assert.AreEqual(true, gameMap.map[randomGen.Next(0, gameMap.map.GetLength(0)), randomGen.Next(0, gameMap.map.GetLength(1))] != 0 &&
                                  gameMap.map[randomGen.Next(0, gameMap.map.GetLength(0)), randomGen.Next(0, gameMap.map.GetLength(1))] != 0);

            // Variables being set
            Assert.AreEqual(true, gameMap.seed != 0);
            Assert.AreEqual(true, gameMap.mapSizeX != 0);
            Assert.AreEqual(true, gameMap.mapSizeY != 0);
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
        public void getPrintableMapTest()
        {
            Random randomGen = new Random();
            Map gameMap = new Map(randomGen.Next(), 30, 30, 1);
            ConsoleColor[,] mapColors = gameMap.getPrintableMap();

            for (int x = 0; x < gameMap.map.GetLength(0); x++)
                for (int y = 0; y < gameMap.map.GetLength(1); y++)
                {
                    float mapValue = gameMap.map[x, y];

                    ConsoleColor tileColor;
                    if (mapValue < -0.05)
                    {
                        tileColor = ConsoleColor.Blue;
                    }
                    else if (mapValue < 0.0)
                    {
                        tileColor = ConsoleColor.Yellow;
                    }
                    else if (mapValue < 1.0)
                    {
                        tileColor = ConsoleColor.Green;
                    }
                    else if (mapValue < 0.35)
                    {
                        tileColor = ConsoleColor.Gray;
                    }
                    else if (mapValue < 1.0)
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

        [TestMethod()]
        public void getSeenMapTest()
        {
            Assert.Fail();
        }
    }
}