using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Uncoal.Engine;

namespace Uncoal.Tests
{
	[TestClass()]
	public class PointExtensionsTests
	{
		static Random rnd = new Random();

		static Coord min;
		static Coord max;

		[ClassInitialize]
		public static void Start(TestContext t)
		{
			min = new Coord(rnd.Next(), rnd.Next());
			max = new Coord(
				rnd.Next(min.X + 100, min.X + 1000),
				rnd.Next(min.Y + 100, min.Y + 1000));
		}

		[TestMethod()]
		public void PointExtensionsClampLessThanMin()
		{
			Coord vector = new Coord(
				rnd.Next(min.X - 100, min.X - 10),
				rnd.Next(min.Y - 100, min.Y - 10));

			vector.Clamp(min, max);
			Assert.AreEqual(min.X, vector.X);
			Assert.AreEqual(min.Y, vector.Y);
		}

		[TestMethod()]
		public void PointExtensionsClampGreaterThanMax()
		{
			Coord vector = new Coord(
				rnd.Next(max.X + 10, max.X + 100),
				rnd.Next(max.Y + 10, max.Y + 100));

			vector.Clamp(min, max);
			Assert.AreEqual(max.X, vector.X);
			Assert.AreEqual(max.Y, vector.Y);
		}

		[TestMethod()]
		public void CoordToCoordF()
		{
			Coord testPoint = new Coord(rnd.Next(), rnd.Next());
			CoordF testVector = (CoordF)testPoint;

			Assert.AreEqual(testVector.X, testPoint.X);
			Assert.AreEqual(testVector.Y, testPoint.Y);
		}
	}
}