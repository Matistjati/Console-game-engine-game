using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Console_game
{
	internal class ReflectiveHelper<T> where T : class
	{
		// This is black magic
		// Which i will try to explain

		public int ClassCount { get; internal set; }

		readonly IEnumerable<Type> TChildren;

		public ReflectiveHelper()
		{
			TChildren = Assembly.GetAssembly(typeof(T)).GetTypes()
				.Where(myType => myType.IsClass && !myType.IsAbstract && myType.IsSubclassOf(typeof(T)));

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
						instances.Add((T)Activator.CreateInstance(instanceType));
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

		public Dictionary<Component, MethodInfo> GetComponentMethodAndInstance(string methodName)
		{
			return GetComponentMethodAndInstance(methodName, defaultBindingFlags);
		}

		public Dictionary<Component, MethodInfo> GetComponentMethodAndInstance(string methodName, BindingFlags bindingFlags)
		{
			if (typeof(T) != typeof(GameObject))
			{
				throw new ArgumentOutOfRangeException("This method only works when T is GameObject");
			}

			Dictionary<Component, MethodInfo> methodAndComponent = new Dictionary<Component, MethodInfo>();
			foreach (T gameObject in TInstances)
			{
				foreach (Component component in (gameObject as GameObject).components)
				{
					if (TryGetMethodFromComponent(component, methodName, bindingFlags, out MethodInfo method))
					{
						methodAndComponent.Add(component, method);
					}
				}
			}

			return methodAndComponent;
		}

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