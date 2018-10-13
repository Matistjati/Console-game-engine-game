using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Threading;

namespace Console_game.Tests
{
    [TestClass]
    public class FrameRunnerTests
    {
        private static float totalTime;
        private static void TestTimeAccuracy()
        {
            totalTime += GameObject.TimeDelta;
        }

        static DateTime start;

        [TestMethod]
        public void FrameRunnerTimePauseResumeTest()
        {
            // This test is slow as we need to sleep for it to work

            FrameRunner.AddFrameSubscriber(TestTimeAccuracy);
            start = DateTime.Now;
            // Let's avoid getting in an infinite loop, shall we?
            new Thread(() =>
            {
                // Make this value higher for a better test
                Thread.Sleep(17 * 3);
                FrameRunner.Pause();
            }).Start();
            FrameRunner.Run();

            FrameRunner.RemoveFrameSubscriber(TestTimeAccuracy);

            // Make this value higher for a better test
            Thread.Sleep(17);
            new Thread(() =>
            {
                Thread.Sleep(1);
                FrameRunner.Pause();
            }).Start();
            FrameRunner.Run();

            double time = Math.Round(totalTime + 0.0017, 3) - Math.Round(GameObject.Time, 3);

            Assert.IsTrue(time <= 0.002);
        }
    }
}
