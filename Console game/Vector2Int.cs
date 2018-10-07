using System;
using System.Numerics;

namespace Console_game
{
    public class Vector2Int
    {
        private int x;
        public int X
        {
            set
            {
                if (value < 0)
                    throw new ArgumentException($"The map start at (0, 0). X was {value}");
                x = value;
            }
            get => x;
        }

        private int y;
        public int Y
        {
            set
            {
                if (value < 0)
                    throw new ArgumentException($"The map start at (0, 0). Y was {value}");
                y = value;
            }
            get => y;
        }

        public Vector2Int() : this(0, 0) { }

        public Vector2Int(int X, int Y)
        {
            x = X;
            y = Y;
        }

        public static implicit operator Vector2(Vector2Int vector)
        {
            return new Vector2(vector.X, vector.Y);
        }

        public static bool operator ==(Vector2Int firstVector, Vector2Int secondVector)
        {
            if (ReferenceEquals(firstVector, secondVector))
            {
                return true;
            }

            if (firstVector is null)
                return false;

            if (secondVector is null)
                return false;

            return (firstVector.X == secondVector.X 
                && firstVector.Y == secondVector.Y);
        }

        public static bool operator !=(Vector2Int firstVector, Vector2Int secondVector)
        {
            return !(firstVector == secondVector);
        }

        public bool Equals(Vector2Int other)
        {
            if (other is null)
                return false;

            if (ReferenceEquals(this, other))
                return true;

            return this.X == other.X && this.Y == other.Y;
        }

        public override bool Equals(object obj)
        {
            Vector2Int item = obj as Vector2Int;
            if (item is null)
                return false;

            return Equals(item);
        }

        public override int GetHashCode()
        {
            return x.GetHashCode();
        }

        public void Clamp(Vector2Int min, Vector2Int max)
        {
            int x;
            x = (this.X > max.X) ? max.X : this.X;
            x = (this.X < min.X) ? min.X : this.X;

            int y;
            y = (this.Y > max.Y) ? max.Y : this.Y;
            y = (this.Y < min.Y) ? min.Y : this.Y;

            this.X = x;
            this.Y = y;
        }
    }
}
