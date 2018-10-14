using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading;
using System.Linq;

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

                foreach (KeyValuePair<MethodInfo, GameObject> method in frameCallback)
                {
                    method.Key.Invoke(method.Value, new object[0]);
                }



                Thread.Sleep(frameWait);
            }
        }
		
		static Dictionary<MethodInfo, GameObject> frameCallback = new Dictionary<MethodInfo, GameObject>();

        internal static void AddFrameSubscriber(KeyValuePair<MethodInfo, GameObject> method)
        {
            if (!frameCallback.Keys.Contains(method.Key))
            {
                frameCallback.Add(method.Key, method.Value);
            }
        }

        internal static void AddFrameSubscriber(Dictionary<MethodInfo, GameObject> method)
        {
            foreach (KeyValuePair<MethodInfo, GameObject> methodInfo in method)
            {
                if (!frameCallback.Keys.Contains(methodInfo.Key))
                {
                    frameCallback.Add(methodInfo.Key, methodInfo.Value);
                }
            }
        }

        internal static void AddFrameSubscriber(MethodInfo method, object instance)
        {
            if (!frameCallback.Keys.Contains(method))
            {
                frameCallback.Add(method, (GameObject)instance);
            }
        }

        internal static void Unsubscribe(MethodInfo method)
        {
            if (frameCallback.Keys.Contains(method))
            {
                frameCallback.Remove(method);
            }
        }

        public static void UnsubscribeAll()
        {
            frameCallback.Clear();
        }
    }
}
