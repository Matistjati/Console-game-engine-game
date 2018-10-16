using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Console_game
{
    internal static class FrameRunner
    {
        static DateTime lastFrameCall;
        static DateTime start;

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

                foreach (KeyValuePair<MethodInfo, Component> method in frameCallback)
                {
                    method.Value.Invoke(method.Key);
                }


            }
        }

        static Dictionary<MethodInfo, Component> frameCallback = new Dictionary<MethodInfo, Component>();

        internal static void AddFrameSubscriber(KeyValuePair<MethodInfo, Component> method)
        {
            if (!frameCallback.Keys.Contains(method.Key))
            {
                frameCallback.Add(method.Key, method.Value);
            }
        }

        internal static void AddFrameSubscriber(Dictionary<MethodInfo, Component> method)
        {
            foreach (KeyValuePair<MethodInfo, Component> methodInfo in method)
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
                frameCallback.Add(method, (Component)instance);
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
