using System;
using System.Threading;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Uncoal.Engine;
using Uncoal.Internal;
using Uncoal.Runner;

namespace Uncoal.Tests
{
	public class SampleComponentTest : Component
	{
		public float total;
		public void TestTimeAccuracy()
		{
			total += GameObject.TimeDelta;
		}
	}


	[TestClass]
	public class GameObjectTests
	{
		static DateTime start;

		[TestMethod]
		public void GameObjectTestTimeAccuracy()
		{

			// This test is slow as we need to sleep for it to work
			SampleComponentTest gameObject = new SampleComponentTest();

			Action methodInfo = ReflectiveHelper<GameObject>.GetAction("TestTimeAccuracy", gameObject);

			FrameRunner.AddFrameSubscriber(methodInfo);
			start = DateTime.Now;

			// Let's avoid getting in an infinite loop, shall we?
			new Thread(() =>
			{
				// Make this value higher for a better test
				Thread.Sleep(17 * 3);
				FrameRunner.Pause();
			}).Start();
			FrameRunner.Run();


			Assert.IsTrue(Math.Round((start - DateTime.Now).TotalSeconds, 3) - Math.Round(gameObject.total, 3) < 0.001);
			Assert.IsTrue(Math.Round((start - DateTime.Now).TotalSeconds, 3) - Math.Round(GameObject.Time, 3) < 0.001);
		}
	}
}
