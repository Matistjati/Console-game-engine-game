using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Threading;

namespace Console_game.Tests
{
    [TestClass]
    public class GameObjectTests
    {
        private static float total;
        private static void TestTimeAccuracy()
        {
            total += GameObject.TimeDelta;
        }

        static DateTime start;

        [TestMethod]
        public void GameObjectTestTimeAccuracy()
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


            Assert.IsTrue(Math.Round((start - DateTime.Now).TotalSeconds, 3) - Math.Round(total, 3) < 0.001);
            Assert.IsTrue(Math.Round((start - DateTime.Now).TotalSeconds, 3) - Math.Round(GameObject.Time, 3) < 0.001);
        }
    }
}
