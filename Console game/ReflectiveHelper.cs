using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace Console_game
{
    internal class ReflectiveHelper<T> where T : class
    {
        // This is black magic

        readonly IEnumerable<Type> gameObjectChildren;
        List<Type> implementsStart = new List<Type>();
        List<Type> implementsUpdate = new List<Type>();

        List<Type> instances = new List<Type>();

        public ReflectiveHelper()
        {
            gameObjectChildren = Assembly.GetAssembly(typeof(T)).GetTypes()
                .Where(myType => myType.IsClass && !myType.IsAbstract && myType.IsSubclassOf(typeof(T)));
        }

        // For this to work, the class must inherit from GameObject and the method must be public and static
        public Dictionary<MethodInfo, T> GetMethodsByString(string methodName)
        {
            Dictionary<MethodInfo, T> methods = new Dictionary<MethodInfo, T>();
            
            foreach (Type instanceType in gameObjectChildren)
            {
                MethodInfo method = instanceType.GetMethod(methodName,
                    BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.IgnoreCase | BindingFlags.Instance);

                if (method is null)
                    continue;

                if (method.ReturnType == typeof(void) && method.GetParameters().Length == 0)
                {
                    methods.Add(method, (T)Activator.CreateInstance(instanceType));
                }
            }

            return methods;
        }

        public static KeyValuePair<MethodInfo, T> GetMethodInfo(Action method, bool staticMethod = false)
        {
            MethodInfo methodInfo = typeof(T).GetMethod(method.Method.Name,
                BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.IgnoreCase | BindingFlags.Instance | ((staticMethod) ? BindingFlags.Static : 0));

            if (methodInfo is null)
                throw new Exception("method not found");

            return new KeyValuePair<MethodInfo, T>(methodInfo, (T)Activator.CreateInstance(typeof(T)));
        }
    }
}