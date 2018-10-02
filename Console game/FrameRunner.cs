using System;
using System.Threading;

namespace Console_game
{
    public static class FrameRunner
    {
        static DateTime lastFrameCall;
        static DateTime start;
		
		private static readonly TimeSpan frameWait = new TimeSpan(166667);


        public static void Pause()
        {
			run = false;
        }

        public static void Run()
        {
			start = DateTime.Now;
			lastFrameCall = DateTime.Now;
            while (true)
            {
                // Calculating and setting timedelta
                GameObject._timeDelta = (float)(DateTime.Now - lastFrameCall).TotalSeconds;

                GameObject._time = (float)(DateTime.Now - start).TotalSeconds;

                Input.UpdateInput();

                lastFrameCall = DateTime.Now;
                frameCallback.Invoke();


                System.Threading.Thread.Sleep(frameWait);
            }
        }
		
		static Globals.GameMethodSignature frameCallback;

        internal static void AddFrameSubscriber(Globals.GameMethodSignature method)
        {
            frameCallback += method;
        }

		private static bool run;

		public static void Start()
        {
			if (!run)
			{
				run = true;
				Run();
			}
        }
    }
}
