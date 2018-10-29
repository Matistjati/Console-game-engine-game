using System;
using System.Threading;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Uncoal.Engine;
using Uncoal.Internal;

namespace Uncoal.Tests
{
	[TestClass]
	public class FrameRunnerTests
	{
		static DateTime start;

		[TestMethod]
		public void FrameRunnerTimePauseResumeTest()
		{
			// This test is slow as we need to sleep for it to work

			SampleComponentTest sampleGameObj = new SampleComponentTest();
			Action action = ReflectiveHelper<GameObject>.GetAction("TestTimeAccuracy", sampleGameObj);

			FrameRunner.UnsubscribeAll();
			FrameRunner.AddFrameSubscriber(action);

			start = DateTime.Now;
			// Let's avoid getting in an infinite loop, shall we?
			new Thread(() =>
			{
				// Make this value higher for a better test
				Thread.Sleep(17 * 3);
				FrameRunner.Pause();
			}).Start();
			FrameRunner.Run();

			FrameRunner.Unsubscribe(action);

			// Make this value higher for a better test
			Thread.Sleep(17);
			new Thread(() =>
			{
				Thread.Sleep(1);
				FrameRunner.Pause();
			}).Start();
			FrameRunner.Run();

			double time = Math.Round(sampleGameObj.total + 0.0017, 3) - Math.Round(GameObject.Time, 3);

			Assert.IsTrue(time <= 0.003);
		}
	}
}
