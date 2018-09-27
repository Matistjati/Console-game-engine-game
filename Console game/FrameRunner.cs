using System;
using System.Threading;

namespace Console_game
{
    public class FrameRunner
    {
        private static class InternalFrameRunner
        {
            public static DateTime lastFrameCall;

            public static AutoResetEvent frameSyncer;
            public static TimerCallback frameCallback;
            public static Timer timeTracker;
            public static Timer frameCaller;

            static public void SetFrameTime(object state)
            {
                // Calculating timedelta and incrementing time
                TimeSpan timeDeltaTimeSpan = DateTime.Now - lastFrameCall;
                float timeDelta = (float)timeDeltaTimeSpan.Ticks / 10000000;
                GameObject.timeDelta = timeDelta;
                GameObject.time += timeDelta;

                lastFrameCall = DateTime.Now;
                timeTracker.Change(new TimeSpan(166667), new TimeSpan(-1));
            }

            private static void EmptyMethod(object state) { }

            static InternalFrameRunner()
            {
                frameCallback = new TimerCallback(EmptyMethod);
                frameSyncer = new AutoResetEvent(false);
                lastFrameCall = DateTime.Now;
            }
        }

        public void Pause()
        {
            InternalFrameRunner.timeTracker.Change(Timeout.Infinite, Timeout.Infinite);
            InternalFrameRunner.frameCaller.Change(Timeout.Infinite, Timeout.Infinite);
        }

        public void Run()
        {
            InternalFrameRunner.timeTracker.Change(new TimeSpan(0), new TimeSpan(-1));
            InternalFrameRunner.frameCaller.Change(new TimeSpan(0), new TimeSpan(166667));
            InternalFrameRunner.lastFrameCall = DateTime.Now;
        }

        public static FrameRunner operator +(FrameRunner _this, TimerCallback other)
        {
            InternalFrameRunner.frameCallback += other;
            return new FrameRunner();
        }

        public FrameRunner()
        {
            InternalFrameRunner.frameCaller = new Timer(InternalFrameRunner.frameCallback, null, new TimeSpan(0), new TimeSpan(166667));
            InternalFrameRunner.timeTracker = new Timer(InternalFrameRunner.SetFrameTime, null, new TimeSpan(0), new TimeSpan(-1));
        }
    }
}
