using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Console_game
{
    public class Vector2
    {
        public int X;
        public int Y;

        public Vector2() :this(0, 0) { }

        public Vector2(int X, int Y)
        {
            this.X = X;
            this.Y = X;
        }
    }
}
