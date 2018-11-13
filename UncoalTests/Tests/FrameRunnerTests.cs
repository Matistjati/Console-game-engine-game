using System;
using System.Threading;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Uncoal.Engine;
using Uncoal.Internal;
using Uncoal.Runner;

namespace Uncoal.Tests
{
	[TestClass]
	public class FrameRunnerTests
	{
		static DateTime start;

		const int sleepTime = 17;

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
				Thread.Sleep(sleepTime * 3);
				FrameRunner.Pause();
			}).Start();

			FrameRunner.Run();

			FrameRunner.Unsubscribe(action);

			// Make this value higher for a better test
			Thread.Sleep(sleepTime);

			new Thread(() =>
			{
				Thread.Sleep(1);
				FrameRunner.Pause();
			}).Start();

			FrameRunner.Run();


			double time = Math.Round(sampleGameObj.total + sleepTime / 10000f, 3) - Math.Round(GameObject.Time, 3);

			Assert.IsTrue(time <= 0.003);
		}
	}
}
