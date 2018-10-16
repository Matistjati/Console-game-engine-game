using System;
using System.Collections.Generic;

namespace Console_game
{
    public class GameObject
    {
        public Component physicalState { get; set; } = new PhysicalState();

        internal List<Component> Components { get; set; } = new List<Component>();

        public Component GetComponent<T>() where T : Component
        {
            Type T_type = typeof(T);
            foreach (Component component in Components)
            {
                if (T_type == component.GetType())
                {
                    return component;
                }
            }
            return null;
        }

        internal static float _timeDelta;
		internal static float _time;

        public static float TimeDelta => _timeDelta;

        public static float Time => _time;
    }
}
