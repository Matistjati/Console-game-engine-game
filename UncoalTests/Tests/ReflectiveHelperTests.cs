using System;
using System.Collections.Generic;
using System.Reflection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Uncoal.Engine;
using Uncoal.Internal;
using Uncoal.Test;

namespace Uncoal.Tests
{
	[TestClass]
	public class ReflectiveHelperTests
	{
		static ReflectiveHelper<GameObject> gameObjects;

		[ClassInitialize]
		public static void ReflectiveHelperConstructorTest(TestContext t)
		{
			gameObjects = new ReflectiveHelper<GameObject>();
			Assert.IsFalse(gameObjects.ClassCount == 0, "No gameobjects found in constructor");
		}

		[TestMethod]
		public void ReflectiveHelperGetTinstanceGameObjectTest()
		{
			List<GameObject> localGameObjects = gameObjects.GetTInstanceNonPrefab();
			Assert.IsFalse(localGameObjects.Count == 0);
			if (localGameObjects[0].GetType() == typeof(ComponentTest))
			{
				Assert.IsTrue(localGameObjects[0].GetComponent<ComponentTest>() != null);
			}
		}

		[TestMethod]
		public void ReflectiveHelperTryGetMethodFromComponentTestSuccess()
		{
			List<GameObject> localGameObjects = gameObjects.GetTInstanceNonPrefab();

			Component componentTest = localGameObjects[0].GetComponent<ComponentTest>();
			bool methodFound = ReflectiveHelper<GameObject>.TryGetMethodFromComponent(
				componentTest,
				"methodfound",
				out MethodInfo method);

			Assert.IsTrue(methodFound, "No method found");
			Assert.IsFalse(method is null);

			Assert.IsTrue((bool)method.Invoke(componentTest, null));
		}

		[TestMethod]
		[ExpectedException(typeof(ArgumentOutOfRangeException))]
		public void ReflectiveHelperGetComponentMethodAndInstanceTestThrow()
		{
			ReflectiveHelper<ReflectiveHelperTests> expectedEmpty = new ReflectiveHelper<ReflectiveHelperTests>();
			Assert.AreEqual(0, expectedEmpty.ClassCount, "There is a class inheriting from ReflectiveHelperTests");

			expectedEmpty.GetComponentAction("sample");
		}

		// Todo passing wrong arg for exception 
		[TestMethod]
		public void ReflectiveHelperGetComponentMethodAndInstanceTestSuccess()
		{
			Action action = gameObjects.GetComponentAction("update");
			Assert.IsTrue(action.GetInvocationList().Length > 1, "Less than one components on GameObjectTest Implements update");
			action.Invoke();
		}


		[TestMethod]
		public void ReflectiveHelperGetMethodInfoTestSuccess()
		{
			List<GameObject> localGameObjects = gameObjects.GetTInstanceNonPrefab();
			MethodInfo method = ReflectiveHelper<Type>.GetMethodInfo<ComponentTest>("methodFound");
			Assert.IsTrue((bool)method.Invoke(localGameObjects[0].GetComponent<ComponentTest>(), null));
		}
	}
}
