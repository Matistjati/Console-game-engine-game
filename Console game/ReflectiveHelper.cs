using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Console_game
{
    public class ReflectiveHelper<T> where T : class
    {
        // This is black magic

        readonly IEnumerable<Type> gameObjectChildren;
        public ReflectiveHelper()
        {
            gameObjectChildren = Assembly.GetAssembly(typeof(T)).GetTypes()
                .Where(myType => myType.IsClass && !myType.IsAbstract && myType.IsSubclassOf(typeof(T)));
        }

        public delegate void methodSignature();

        // For this to work, the class must inherit from GameObject and the method must be public and static
        public methodSignature GetMethodsByString(string methodName)
        {
            List<Delegate> methods = new List<Delegate>();
            
            foreach (Type instanceType in gameObjectChildren)
            {
                if (instanceType.GetMethod(methodName) != null)
                {
                    methods.Add(Delegate.CreateDelegate(typeof(methodSignature), instanceType, methodName, false, true));
                }
            }

            methodSignature joinedSubscribers = new methodSignature(DoNothing);
            for (int i = 0; i < methods.Count; i++)
            {
                joinedSubscribers += (methodSignature)methods[i];
            }
            return joinedSubscribers;
        }

        public void DoNothing()
        { }
    }
}