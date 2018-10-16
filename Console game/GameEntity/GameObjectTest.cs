using System;
using System.Collections.Generic;

namespace Console_game
{
    class GameObjectTest : GameObject
    {
        public GameObjectTest()
        {
            Components.Add(physicalState);
            Components.Add(new ComponentTest());
            Components.Add(new SampleComponent());
        }
    }
}
