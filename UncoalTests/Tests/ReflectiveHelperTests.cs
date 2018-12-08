using System;
using System.Collections.Generic;
using System.Reflection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Uncoal.Engine;
using Uncoal.Internal;
using static Uncoal.Tests.Extensions;

namespace Uncoal.Tests
{
	class TestComponent : Component
	{
		void update()
		{

		}

		bool Methodfound()
		{
			return true;
		}
	}

	class TestGameObj : GameObject
	{
		public TestGameObj()
		{
			AddComponent<TestComponent>();
		}
	}

	static class Extensions
	{
		public static bool HasType<TDeclare, TSearch>(this List<TDeclare> items, out int index)
		{
			for (int i = 0; i < items.Count; i++)
			{
				if (items[i].GetType() == typeof(TSearch))
				{
					index = i;
					return true;
				}
			}
			index = -1;
			return false;
		}
	}

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

			if (localGameObjects.HasType<GameObject, TestGameObj>(out int index))
			{
				Assert.IsTrue(localGameObjects[index].GetComponent<TestComponent>() != null);
			}
			else
			{
				Assert.Fail("could not find an object of type testgameobj");
			}
		}

		[TestMethod]
		public void ReflectiveHelperTryGetMethodFromComponentTestSuccess()
		{
			List<GameObject> localGameObjects = gameObjects.GetTInstanceNonPrefab();

			if (localGameObjects.HasType<GameObject, TestGameObj>(out int index))
			{
				Component componentTest = localGameObjects[index].GetComponent<TestComponent>();
				bool methodFound = ReflectiveHelper<GameObject>.TryGetMethodFromComponent(
					componentTest,
					"methodfound",
					out MethodInfo method);

				Assert.IsTrue(methodFound, "No method found");
				Assert.IsFalse(method is null);

				Assert.IsTrue((bool)method.Invoke(componentTest, null));
			}
			else
			{
				Assert.Fail("could not find an object of type testgameobj");
			}
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
			Assert.IsTrue(action.GetInvocationList().Length >= 1, "Less than one components on GameObjectTest Implements update");
			action.Invoke();
		}


		[TestMethod]
		public void ReflectiveHelperGetMethodInfoTestSuccess()
		{
			List<GameObject> localGameObjects = gameObjects.GetTInstanceNonPrefab();
			MethodInfo method = ReflectiveHelper<Type>.GetMethodInfo<TestComponent>("methodFound");
			if (localGameObjects.HasType<GameObject, TestGameObj>(out int index))
			{
				Assert.IsTrue((bool)method.Invoke(localGameObjects[index].GetComponent<TestComponent>(), null));
			}
			else
			{
				Assert.Fail("could not find an object of type testgameobj");
			}
		}
	}
}
