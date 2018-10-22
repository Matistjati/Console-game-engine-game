using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Console_game
{
    class SampleComponent : Component
    {
        int reps = 0;
        void update()
        {
            //physicalState.Position = new CoordF(physicalState.Position.X + 100 * GameObject.TimeDelta, physicalState.Position.Y + 3 * GameObject.TimeDelta);
            if (physicalState.Scale < 1)
            {
                physicalState.Scale = 1.5f;
                reps++;
            }
            physicalState.Scale -= 1f * GameObject.TimeDelta;
            if (reps > 5)
            {
                GameObject.Destroy(gameObject);
            }
        }
    }
}
