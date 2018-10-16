using System;

namespace Console_game
{
    public class PhysicalState : Component
    {
        public CoordF Position { get; set; } = new CoordF();

        private int rotation;
        public int Rotation
        {
            get => rotation;
            set
            {
                if (value > 360)
                {
                    rotation = 360;
                }
                else if (value < 0)
                {
                    rotation = 0;
                }
                rotation = value;
            }
        }

        private float scale = 1f;
        public float Scale
        {
            get => scale;
            set
            {
                if (value <= 0)
                {
                    throw new ArgumentOutOfRangeException($"Scale must be greater than 0. Scale was {value}");
                }
                scale = value;
            }
        }

    }
}
