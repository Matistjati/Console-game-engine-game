using System;
using System.Collections.Generic;

namespace Console_game
{
    class GameObjectTest : GameObject
    {
        public GameObjectTest()
        {
            SpriteDisplayer componentTest = AddComponent<SpriteDisplayer>();
            physicalState.Position = new CoordF(10f, 10f);
            physicalState.Scale = 1f;
            componentTest.SetImage("sample.png");
            SampleComponent sampleTest = AddComponent<SampleComponent>();
        }
    }
}
