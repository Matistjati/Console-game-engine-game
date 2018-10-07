using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace Console_game.Tests
{
    [TestClass()]
    public class Vector2IntTests
    {
        static Random rnd = new Random();

        [TestMethod()]
        public void Vector2IntEmptyConstructor()
        {
            Vector2Int vector = new Vector2Int();
            Assert.AreEqual(0, vector.X);
            Assert.AreEqual(0, vector.Y);
        }

        [TestMethod()]
        public void Vector2IntNormalConstructor()
        {
            int x = rnd.Next();
            int y = rnd.Next();
            Vector2Int vector = new Vector2Int(x, y);
            Vector2Int vectors = new Vector2Int(x, y);

            Assert.AreEqual(x, vector.X);
            Assert.AreEqual(y, vector.Y);
        }

        // No method for != as it builds on ==
        [TestMethod()]
        public void Vector2IntEqualsTrue()
        {
            int x = rnd.Next();
            int y = rnd.Next();
            Vector2Int vector1 = new Vector2Int(x, y);
            Vector2Int vector2 = new Vector2Int(x, y);
            Assert.IsTrue(vector1 == vector2);
            Assert.IsTrue(vector1.Equals(vector2));
            Assert.IsTrue(vector2.Equals(vector1));
        }

        // Some of these might succeed if the random number generator generates the same output
        [TestMethod()]
        public void Vector2IntEqualsFalse()
        {
            Vector2Int vector1 = new Vector2Int(rnd.Next(), rnd.Next());
            Vector2Int vector2 = new Vector2Int(rnd.Next(), rnd.Next());
            Assert.IsFalse(vector1 == vector2);
            Assert.IsFalse(vector1.Equals(vector2));
            Assert.IsFalse(vector2.Equals(vector1));
        }

        [TestMethod()]
        public void Vector2IntEqualsObjTrue()
        {
            int x = rnd.Next();
            int y = rnd.Next();
            Vector2Int vector1 = new Vector2Int(x, y);
            Vector2Int vector2 = new Vector2Int(x, y);

            object vector1obj = vector1;
            object vector2obj = vector2;

            Assert.IsTrue(vector1.Equals(vector1obj));
            Assert.IsTrue(vector1.Equals(vector2obj));
            Assert.IsTrue(vector2.Equals(vector1obj));
            Assert.IsTrue(vector2.Equals(vector2obj));
        }

        [TestMethod()]
        public void Vector2IntEqualsObjFalse()
        {
            Vector2Int vector1 = new Vector2Int(rnd.Next(), rnd.Next());
            Vector2Int vector2 = new Vector2Int(rnd.Next(), rnd.Next());

            object vector1obj = vector1;
            object vector2obj = vector2;

            Assert.IsFalse(vector1.Equals(vector2obj));
            Assert.IsFalse(vector2.Equals(vector1obj));
        }

        [TestMethod()]
        public void Vector2IntGetHashCodeSame()
        {
            int x = rnd.Next();
            int y = rnd.Next();
            Vector2Int vector1 = new Vector2Int(x, y);
            Vector2Int vector2 = new Vector2Int(x, y);
            Assert.AreEqual(vector1.GetHashCode(), vector2.GetHashCode());
        }

        [TestMethod()]
        public void Vector2IntGetHashCodeDifferent()
        {
            Vector2Int vector1 = new Vector2Int(rnd.Next(), rnd.Next());
            Vector2Int vector2 = new Vector2Int(rnd.Next(), rnd.Next());
            Assert.AreNotEqual(vector1.GetHashCode(), vector2.GetHashCode());
        }

        static Vector2Int min;
        static Vector2Int max;

        [ClassInitialize]
        public static void Start(TestContext t)
        {
            min = new Vector2Int(rnd.Next(), rnd.Next());
            max = new Vector2Int(rnd.Next(min.X + 100, min.X + 1000), rnd.Next(min.Y + 100, min.Y + 1000));
        }


        [TestMethod()]
        public void Vector2IntClampLessThanMin()
        {
            Vector2Int vector = new Vector2Int(
                rnd.Next(min.X -100, min.X - 10),
                rnd.Next(min.Y - 100, min.Y - 10));
            vector.Clamp(min, max);
            Assert.AreEqual(min.X, vector.X);
            Assert.AreEqual(min.Y, vector.Y);
        }
    }
}