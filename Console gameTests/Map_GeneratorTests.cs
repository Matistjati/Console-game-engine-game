using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace Console_game.Tests
{
    [TestClass()]
    public class Map_GeneratorTests
    {
        [TestMethod()]
        public void MakeMapSuccessNormal()
        {
            Random randomGen = new Random();
            float[,] gameMap = Map_Generator.MakeMap(randomGen.Next(), randomGen.Next(1, 100), randomGen.Next(1, 25), 1);

            // Assuring that the map has been filled
            Assert.AreEqual(true, gameMap[randomGen.Next(0, gameMap.GetLength(0)), randomGen.Next(0, gameMap.GetLength(1))] != 0 &&
                                  gameMap[randomGen.Next(0, gameMap.GetLength(0)), randomGen.Next(0, gameMap.GetLength(1))] != 0);
        }

        [TestMethod()]
        public void MakeMapSuccessNegativeSeed()
        {
            Random randomGen = new Random();
            float[,] gameMap = Map_Generator.MakeMap(randomGen.Next(-1000, -1), randomGen.Next(1, 100), randomGen.Next(1, 25), 1);

            // Assuring that the map has been filled
            Assert.AreEqual(true, gameMap[randomGen.Next(0, gameMap.GetLength(0)), randomGen.Next(0, gameMap.GetLength(1))] != 0 &&
                                  gameMap[randomGen.Next(0, gameMap.GetLength(0)), randomGen.Next(0, gameMap.GetLength(1))] != 0);
        }

        [TestMethod()]
        [ExpectedException(typeof(ArgumentException))]
        public void MakeMapNegativeXException()
        {
            Random randomGen = new Random();
            float[,] gameMap = Map_Generator.MakeMap(randomGen.Next(), -20, randomGen.Next(1, 25), 1);
        }

        [TestMethod()]
        [ExpectedException(typeof(ArgumentException))]
        public void MakeMapNegativeYException()
        {
            Random randomGen = new Random();
            float[,] gameMap = Map_Generator.MakeMap(randomGen.Next(), randomGen.Next(1, 25), -50, 1);
        }
    }
}