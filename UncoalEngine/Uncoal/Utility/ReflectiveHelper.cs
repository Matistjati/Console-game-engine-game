using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Uncoal.Engine;

namespace Uncoal.Internal
{
	internal class ReflectiveHelper<T> where T : class
	{
		// This is black magic
		// Which i will try to explain
		// Nvm i dont comment my code

		public int ClassCount { get; internal set; }

		public List<Type> TChildren = new List<Type>();

		public ReflectiveHelper()
		{
			// This slow
			Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();
			for (int i = 0; i < assemblies.Length; i++)
			{
				TChildren.AddRange(assemblies[i].GetTypes()
					.Where(myType => myType.IsClass && !myType.IsAbstract && myType.IsSubclassOf(typeof(T))));
			}


			ClassCount = TChildren.Count();
		}

		List<T> TInstances = new List<T>();

		/// <summary>
		/// Instantiates all classes in the assembly that derives from T
		/// </summary>
		/// <returns>
		/// An array instances of classes in the assembly that derive from T
		/// </returns>
		public List<T> GetTInstanceNonPrefab()
		{
			if (TInstances.Count != 0)
			{
				return TInstances;
			}
			else
			{
				List<T> instances = new List<T>();
				foreach (Type instanceType in TChildren)
				{
					if (Attribute.GetCustomAttribute(instanceType, typeof(IsPrefabAttribute)) is null)
					{
						try
						{
							instances.Add((T)Activator.CreateInstance(instanceType));
						}
						catch (MissingMethodException exception)
						{
							MissingMethodException methodEx = new MissingMethodException($"Empty constructor not found in class {instanceType.ToString()}", exception);
							throw methodEx;
						}
						catch (TargetInvocationException exception)
						{
							TargetInvocationException targetEx = new TargetInvocationException($"Exception was thrown during creation of class {instanceType.ToString()}", exception);
							throw targetEx;
						}
					}
				}
				TInstances = instances;
				return instances;
			}

		}

		const BindingFlags defaultBindingFlags = BindingFlags.Public |
			BindingFlags.NonPublic |
			BindingFlags.Instance |
			BindingFlags.IgnoreCase;

		/// <summary>
		/// Instantiates all classes in the assembly that derives from T and implements a method
		/// </summary>
		/// <returns>
		/// An array instances of classes in the assembly that derive from T and implements a method
		/// </returns>
		/// <param name="component">The component to get the methodinfo from.</param>
		/// <param name="methodName">The method that the classes need to implement.</param>
		/// <param name="methodInfo">The methodinfo that will be supplied if the components implements the method.</param>
		public static bool TryGetMethodFromComponent(Component component, string methodName, out MethodInfo methodInfo)
		{
			return TryGetMethodFromComponent(component, methodName, defaultBindingFlags, out methodInfo);
		}

		/// <summary>
		/// Instantiates all classes in the assembly that derives from T and implements a method
		/// </summary>
		/// <returns>
		/// An array instances of classes in the assembly that derive from T and implements a method
		/// </returns>
		/// <param name="component">The component to get the methodinfo from.</param>
		/// <param name="methodName">The method that the classes need to implement.</param>
		/// <param name="methodInfo">The methodinfo that will be supplied if the components implements the method.</param>
		public static bool TryGetMethodFromComponent(Component component, string methodName,
			BindingFlags bindingFlags, out MethodInfo methodInfo)
		{
			MethodInfo ComponentMethodInfo = component.GetType().GetMethod(methodName, defaultBindingFlags);
			methodInfo = ComponentMethodInfo;

			return (ComponentMethodInfo is null) ? false : true;
		}

		public Action GetComponentAction(string methodName)
		{
			return GetComponentAction(methodName, defaultBindingFlags);
		}

		private void DoNothing() { }

		public Action GetComponentAction(string methodName, BindingFlags bindingFlags)
		{
			if (typeof(T) != typeof(GameObject))
			{
				Log.DefaultLogger.LogError($"T in reflectivehlper: {typeof(T)} was not gameobject");
				throw new ArgumentOutOfRangeException("This method only works when T is GameObject");
			}

			Action methods = new Action(DoNothing);

			foreach (T gameObject in TInstances)
			{
				foreach (Component component in (gameObject as GameObject).components)
				{
					if (TryGetMethodFromComponent(component, methodName, bindingFlags, out MethodInfo method))
					{
						Action action = (Action)method.CreateDelegate(typeof(Action), component);
						if (action is null)
						{
							// Handle??
						}
						methods += action;
					}
				}
			}

			methods -= DoNothing;
			return methods;
		}

		public static Action GetAction(string method, object target)
		{
			MethodInfo methodInfo = target.GetType().GetMethod(method,
				defaultBindingFlags);

			return (Action)methodInfo?.CreateDelegate(typeof(Action), target);
		}

		public static Action GetAction(MethodInfo method, object target) => (Action)method.CreateDelegate(typeof(Action), target);


		public static MethodInfo GetMethodInfo<TClassType>(string method)
		{
			MethodInfo methodInfo = typeof(TClassType).GetMethod(method,
				defaultBindingFlags);

			if (methodInfo is null)
				throw new Exception("method not found");

			return methodInfo;
		}

		public static MethodInfo GetMethodInfo<TClassType>(Action method)
		{
			MethodInfo methodInfo = typeof(TClassType).GetMethod(method.Method.Name,
				BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.IgnoreCase | BindingFlags.Instance);

			if (methodInfo is null)
				throw new Exception("method not found");

			return methodInfo;
		}
	}
}