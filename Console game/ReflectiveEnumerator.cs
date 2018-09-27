using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Console_game
{
    public class GameObjectChildren<T>
    {
        // This is black magic

        readonly IEnumerable<Type> gameObjectChildren;
        public GameObjectChildren()
        {
            gameObjectChildren = Assembly.GetAssembly(typeof(T)).GetTypes()
                .Where(myType => myType.IsClass && !myType.IsAbstract && myType.IsSubclassOf(typeof(T)));
        }

        public delegate void frameCallSubscribers();

        // For this to work, the class must inherit from GameObject and the method must be public and static
        public frameCallSubscribers GetMethodsByString(string methodName)
        {
            List<Delegate> methods = new List<Delegate>();
            
            foreach (Type instanceType in gameObjectChildren)
            {
                if (instanceType.GetMethod(methodName) != null)
                {
                    methods.Add(Delegate.CreateDelegate(typeof(frameCallSubscribers), instanceType, methodName, false, true));
                }
            }

            frameCallSubscribers joinedSubscribers = new frameCallSubscribers(DoNothing);
            for (int i = 0; i < methods.Count; i++)
            {
                joinedSubscribers += (frameCallSubscribers)methods[i];
            }
            return joinedSubscribers;
        }

        public void DoNothing()
        { }
    }
}