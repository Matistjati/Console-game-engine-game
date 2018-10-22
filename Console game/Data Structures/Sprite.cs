using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;

namespace Console_game
{
    class Sprite
    {
        public readonly Color[,] colorValues;
        public readonly Coord Size;
        // This assumes that the fontsize is 6 * 10
        readonly Coord standardFontSize = new Coord(6, 10);
        public readonly float Scale;


        public Sprite(string image) : this((Bitmap)Image.FromFile(image), 1f)
        { }

        public Sprite(Image image) : this(new Bitmap(image), 1f)
        { }

        public Sprite(Bitmap image, float scale)
        {
            if (scale != 1)
            {
                Log.DefaultLogger.LogInfo(scale);
                image = ResizeImage(image, new Size((int)(image.Width * scale), (int)(image.Width * scale)));
            }

            Scale = scale;
            Size = new Coord(image.Width, image.Height);
            colorValues = new Color[Size.X, Size.Y];

            for (int x = 0; x < Size.X; x++)
            {
                for (int y = 0; y < Size.Y; y++)
                {
                    colorValues[x, y] = image.GetPixel(x, y);
                }
            }

            if (scale != 1)
            {
                image.Dispose();
            }
        }

        public Sprite(Color[,] image, char printedChar)
        {
            Size = new Coord(image.GetLength(0), image.GetLength(1));

            colorValues = image;
        }

        public static Bitmap ResizeImage(Bitmap imgToResize, Size size)
        {
            if (size.Width <= 0 && size.Height <= 0)
                throw new ArgumentException($"Size was less than or equal to zero. Size was X: {size.Width} Y: {size.Height}");

            return new Bitmap(imgToResize, size);
        }
    }
}
