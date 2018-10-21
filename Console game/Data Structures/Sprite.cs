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


        public Sprite(string image, char printedChar) : this((Bitmap)Image.FromFile(image), printedChar, 1f)
        { }

        public Sprite(Image image, char printedChar) : this(new Bitmap(image), printedChar, 1f)
        { }

        public Sprite(Bitmap image, char printedChar, float scale)
        {
            if (scale != 1)
            {
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

            image.Dispose();
        }

        public Sprite(Color[,] image, char printedChar)
        {
            Size = new Coord(image.GetLength(0), image.GetLength(1));

            colorValues = image;
        }

        public static Bitmap ResizeImage(Bitmap imgToResize, Size size)
        {
            return new Bitmap(imgToResize, size);
        }
    }
}
