using System;
using System.Collections.Generic;

namespace Console_game
{
    class GameObjectTest : GameObject
    {
        public GameObjectTest()
        {
            SpriteDisplayer componentTest = AddComponent<SpriteDisplayer>();
            physicalState.Scale = 1.5f;
            componentTest.SetImage("sample.png");
            SampleComponent sampleTest = AddComponent<SampleComponent>();
        }
    }
}
