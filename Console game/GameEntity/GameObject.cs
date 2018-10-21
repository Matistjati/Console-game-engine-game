using System;
using System.Collections.Generic;
using System.Linq;

namespace Console_game
{
    public class GameObject
    {
        public PhysicalState physicalState { get; set; } = new PhysicalState();

        internal List<Component> components = new List<Component>();

        public void AddComponent(Component component)
        {
            component.physicalState = physicalState;
            component.gameObject = this;
            components.Add(component);
        }

        public TComponent AddComponent<TComponent>() where TComponent : Component
        {
            if (!components.OfType<TComponent>().Any())
            {
                Component newComponent = (Component)Activator.CreateInstance(typeof(TComponent));
                newComponent.physicalState = physicalState;
                newComponent.gameObject = this;
                components.Add(newComponent);
                return (TComponent)newComponent;
            }
            else
            {
                throw new ArgumentException($"There already exists another component of type {typeof(TComponent)} on this gameObject");
            }
        }

        public T GetComponent<T>() where T : Component
        {
            Type T_type = typeof(T);
            foreach (Component component in components)
            {
                if (T_type == component.GetType())
                {
                    return (T)component;
                }
            }
            return null;
        }

        public bool HasComponent<T>() where T : Component
        {
            return (!components.OfType<T>().Any()) ? true : false;
        }

        public GameObject Instantiate<TPrefab>() where TPrefab : GameObject
        {
            if (!(Attribute.GetCustomAttribute(typeof(TPrefab), typeof(IsPrefabAttribute)) is null))
            {
                return (GameObject)Activator.CreateInstance(typeof(TPrefab));
            }
            else
            {
                throw new ArgumentException("TPrefab must be marked with the IsPrefab attribute");
            }
        }

        internal static float _timeDelta;
        internal static float _time;

        public static float TimeDelta => _timeDelta;

        public static float Time => _time;
    }
}
