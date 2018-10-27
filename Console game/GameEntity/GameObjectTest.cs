using System;
using System.Collections.Generic;

namespace Console_game
{
	class GameObjectTest : GameObject
	{
		int GameObjectTestt;
		public GameObjectTest()
		{
			SpriteDisplayer sprite = AddComponent<SpriteDisplayer>();
			physicalState.Scale = 1.5f;
			sprite.SetImage("sample.png");
			sprite.Layer = 0;

			SampleComponent sampleTest = AddComponent<SampleComponent>();
		}
	}
}
