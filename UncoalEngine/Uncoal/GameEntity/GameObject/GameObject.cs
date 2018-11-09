using System;
using System.Reflection;
using System.Collections.Generic;
using System.Linq;
using Uncoal.Internal;
using Uncoal.Runner;

namespace Uncoal.Engine
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

		public static GameObject Instantiate<TPrefab>() where TPrefab : GameObject
		{
			return Instantiate<TPrefab>(new Coord(0, 0), null);
		}

		public static GameObject Instantiate<TPrefab>(Coord position) where TPrefab : GameObject
		{
			return Instantiate<TPrefab>(position, null);
		}

		public static GameObject Instantiate<TPrefab>(object[] args) where TPrefab : GameObject
		{
			return Instantiate<TPrefab>(new Coord(0, 0), args);
		}

		public static GameObject Instantiate<TPrefab>(Coord location, object[] args) where TPrefab : GameObject
		{
			// Checking if the TPrefab type is marked with the isprefab attribute
			if (!(Attribute.GetCustomAttribute(typeof(TPrefab), typeof(IsPrefabAttribute)) is null))
			{
				// Note:
				// Lots of things here get assigned to null
				// This is to ensure that gameobjects can be destroyed and have their memory freed up during runtime

				// Creating a new instance of the gameobject
				GameObject newGameObject = (GameObject)Activator.CreateInstance(typeof(TPrefab), args);

				// Setting the position of the gameobject
				newGameObject.physicalState.Position = (CoordF)location;

				// Adding physicalstate to the gameobjects components
				newGameObject.AddComponent(newGameObject.physicalState);

				// If the gameobject has a spritedisplayer, add it to RenderedGameObjects
				if (newGameObject.GetComponent<SpriteDisplayer>() is SpriteDisplayer sprite && sprite.IsInitialized)
				{
					FrameRunner.RenderedGameObjects.Add(sprite);
					//Leave no references hanging
					sprite = null;
				}

				// Invoking start
				foreach (Component component in newGameObject.components)
				{
					if (ReflectiveHelper<Type>.TryGetMethodFromComponent(component, "start", out MethodInfo method))
					{
						method.Invoke(component, null);
						// Leave no references hanging
						method = null;
					}
				}

				// Adding update from the gameobjects components to framerunners update list
				void doNothing() { }
				Action update = new Action(doNothing);

				foreach (Component component in newGameObject.components)
				{
					update += ReflectiveHelper<Type>.GetAction("update", component);
				}
				update -= doNothing;

				FrameRunner.AddFrameSubscriber(update);

				// Leave no references hanging
				update = null;
				newGameObject = null;
				return newGameObject;
			}
			else
			{
				throw new ArgumentException($"TPrefab {typeof(TPrefab)} must be marked with [IsPrefab] ");
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
