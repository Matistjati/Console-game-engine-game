using System;
using System.Threading;

namespace Console_game
{
    internal static class FrameRunner
    {
        static DateTime lastFrameCall;
        static DateTime start;
		
		private static readonly TimeSpan frameWait = new TimeSpan(166667);

        static bool run;

        static bool firstRun = true;

        static DateTime pauseStartTime;
        public static void Pause()
        {
			run = false;
            pauseStartTime = DateTime.Now;
        }

        public static void Run()
        {
            if (firstRun)
            {
                start = DateTime.Now;
                firstRun = false;
            }
            else
            {
                start += DateTime.Now - pauseStartTime;
            }

			lastFrameCall = DateTime.Now;

            run = true;
            while (run)
            {
                // Calculating and setting timedelta
                GameObject._timeDelta = (float)(DateTime.Now - lastFrameCall).TotalSeconds;

                GameObject._time = (float)(DateTime.Now - start).TotalSeconds;

                Input.UpdateInput();

                lastFrameCall = DateTime.Now;
                frameCallback?.Invoke();


                Thread.Sleep(frameWait);
            }
        }
		
		static Globals.GameMethodSignature frameCallback;

        internal static void AddFrameSubscriber(Globals.GameMethodSignature method)
        {
            frameCallback += method;
        }

        internal static void RemoveFrameSubscriber(Globals.GameMethodSignature method)
        {
            frameCallback -= method;
        }
    }
}
