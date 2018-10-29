using System;
using System.Collections.Generic;
using System.Linq;
using Uncoal.Internal;

namespace Uncoal.Engine
{
	public class GameObject
	{
		public PhysicalState physicalState { get; set; } = new PhysicalState();

		internal List<Component> components = new List<Component>();

		internal static bool isStartUpPhase = true;

		public void AddComponent(Component component)
		{
			if (isStartUpPhase)
			{
				component.physicalState = physicalState;
				component.gameObject = this;
				components.Add(component);
			}
			else
			{
				throw new Exception("Cannot add a component from a component");
			}
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

		internal void RecalculateSpriteSize()
		{
			if (GetComponent<SpriteDisplayer>() is SpriteDisplayer sprite)
			{
				sprite.RecalculateSpriteSize();
			}
		}

		public static void Destroy(GameObject gameObject)
		{
			// The only way to do this really is to kill all references to the objects
			FrameRunner.destructionQueue.Enqueue(gameObject);
		}

		internal static float _timeDelta;
		internal static float _time;

		public static float TimeDelta => _timeDelta;

		public static float Time => _time;
	}
}
