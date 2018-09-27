using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Console_game
{
    abstract class GameObject
    {
        public volatile static float timeDelta;
        public volatile static float time;

        public virtual void frameUpdate() { }
    }
}
