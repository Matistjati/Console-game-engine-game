using System.Numerics;

namespace Console_game
{
    public class Vector2Int
    {
        public Vector2 floatVector;
        public int X
        {
            set
            {
                floatVector.X = value;
            }
            get
            { return (int)floatVector.X; }
        }

        public int Y
        {
            set
            {
                floatVector.Y = value;
            }
            get
            {
                return (int)floatVector.Y;
            }
        }

        public Vector2Int() : this(0, 0) { }

        public Vector2Int(int X, int Y)
        {
            floatVector = new Vector2(X, Y);
        }
    }
}
