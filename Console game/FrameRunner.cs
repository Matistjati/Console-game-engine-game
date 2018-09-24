using System;
using System.Threading;

namespace Console_game
{
    public static class FrameInfo
    {
        public static float timeDelta;
    }

    public class FrameRunner
    {
        private static class InternalFrameRunner
        {
            static public void SetLastFrameCall(object state)
            {
                TimeSpan timeDeltaTimeSpan = DateTime.Now - lastFrameCall;
                Console.WriteLine((float)timeDeltaTimeSpan.Ticks / 10000000);
                FrameInfo.timeDelta = (float)timeDeltaTimeSpan.Ticks / 10000000;
                lastFrameCall = DateTime.Now;
            }

            static InternalFrameRunner()
            {
                lastFrameCall = new DateTime();
                frameCallback = new TimerCallback(SetLastFrameCall);
            }

            public static DateTime lastFrameCall;

            public static TimerCallback frameCallback;
            public static Timer frameTimer;
        }

        public void Pause()
        {
            InternalFrameRunner.frameTimer.Change(Timeout.Infinite, Timeout.Infinite);
        }

        public void Run()
        {
            InternalFrameRunner.frameTimer.Change(new TimeSpan(0), new TimeSpan(166667));
        }

        public static FrameRunner operator +(FrameRunner _this, TimerCallback other)
        {
            InternalFrameRunner.frameCallback += other;
            return new FrameRunner();
        }

        public FrameRunner()
        {
            InternalFrameRunner.frameTimer = new Timer(InternalFrameRunner.frameCallback, null, new TimeSpan(0), new TimeSpan(166667));
        }
    }
}
