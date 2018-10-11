using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Drawing;
using System.Numerics;

namespace Console_game.Tests
{
    [TestClass()]
    public class PointExtensionsTests
    {
        static Random rnd = new Random();

        [TestMethod()]
        public void PointExtensionsToVector2()
        {
            Point testPoint = new Point(rnd.Next(), rnd.Next());
            Vector2 testVector = testPoint.ToVector2();

            Assert.AreEqual(testVector.X, testPoint.X);
            Assert.AreEqual(testVector.Y, testPoint.Y);
        }

        [TestMethod()]
        public void PointExtensionsToPoint()
        {
            Vector2 testVector = new Vector2(rnd.Next(), rnd.Next());
            Point testPoint = testVector.ToPoint();

            Assert.AreEqual(testVector.X, testPoint.X);
            Assert.AreEqual(testVector.Y, testPoint.Y);
        }


        static Point min;
        static Point max;

        [ClassInitialize]
        public static void Start(TestContext t)
        {
            min = new Point(rnd.Next(), rnd.Next());
            max = new Point(rnd.Next(min.X + 100, min.X + 1000), rnd.Next(min.Y + 100, min.Y + 1000));
        }

        [TestMethod()]
        public void PointExtensionsClampLessThanMin()
        {
            Point vector = new Point(
                rnd.Next(min.X - 100, min.X - 10),
                rnd.Next(min.Y - 100, min.Y - 10));

            vector = vector.Clamp(min, max);
            Assert.AreEqual(min.X, vector.X);
            Assert.AreEqual(min.Y, vector.Y);
        }

        [TestMethod()]
        public void PointExtensionsClampGreaterThanMax()
        {
            Point vector = new Point(
                rnd.Next(max.X + 10, max.X + 100),
                rnd.Next(max.Y + 10, max.Y + 100));

            vector = vector.Clamp(min, max);
            Assert.AreEqual(max.X, vector.X);
            Assert.AreEqual(max.Y, vector.Y);
        }
    }
}