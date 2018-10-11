using System;
using System.Drawing;
using System.Numerics;

namespace Console_game
{
    public static class PointExtensions
    {
        public static Point Clamp(this Point point, Point min, Point max)
        {
            int X = point.X;
            X = (point.X > max.X) ? max.X : X;
            X = (point.X < min.X) ? min.X : X;

            int Y = point.Y;
            Y = (point.Y > max.Y) ? max.Y : Y;
            Y = (point.Y < min.Y) ? min.Y : Y;

            return new Point(X, Y);
        }

        public static Vector2 ToVector2(this Point point)
        {
            return new Vector2(point.X, point.Y);
        }

        public static Point ToPoint(this Vector2 point)
        {
            return new Point((int)point.X, (int)point.Y);
        }
    }
}
