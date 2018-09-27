using System;

namespace Console_game
{
    abstract class GameObject
    {
        public volatile static float timeDelta;
        public volatile static float time;
        public virtual void frameUpdate() { }

        public virtual void Update() { }
    }
}
