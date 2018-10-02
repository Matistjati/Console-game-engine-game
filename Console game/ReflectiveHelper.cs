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

        // For this to work, the class must inherit from GameObject and the method must be public and static
        public Globals.GameMethodSignature GetMethodsByString(string methodName)
        {
            List<Delegate> methods = new List<Delegate>();
            
            foreach (Type instanceType in gameObjectChildren)
            {
                if (instanceType.GetMethod(methodName) != null)
                {
                    methods.Add(Delegate.CreateDelegate(typeof(Globals.GameMethodSignature), instanceType, methodName, false, true));
                }
            }

            Globals.GameMethodSignature joinedSubscribers = new Globals.GameMethodSignature(() => { });
            for (int i = 0; i < methods.Count; i++)
            {
                joinedSubscribers += (Globals.GameMethodSignature)methods[i];
            }
            return joinedSubscribers;
        }
    }
}