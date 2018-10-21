using System.Drawing;

namespace Console_game
{
    class SpriteDisplayer : Component
    {
        public bool IsVisible { get; set; } = true;

        public bool IsInitialized
        {
            get { return !(Sprite.colorValues is null); }
        }


        Sprite Sprite { get; set; }

        // More overloads for setting pic
        // Max length 1
        public string PrintedChar { get; set; } = "█";
        Color[,] colorBase;

        public int Width { get => (int)Sprite.Size.X; }
        public int Heigth { get => (int)Sprite.Size.Y; }

        public Color[,] ColorMap
        {
            get => Sprite.colorValues;
        }

        internal Color[,] ColorBase
        {
            get => colorBase;
            set
            {
                colorBase = value;
                ColorBaseChanged();
            }
        }

        Bitmap imageBase;
        internal Bitmap ImageBase
        {
            get => imageBase;
            set
            {
                imageBase = value;
                ImageBaseChanged();
            }
        }

        public void SetPrintedChar(string printedChar) => PrintedChar = printedChar;

        public void SetImage(Bitmap image) => ImageBase = image;

        public void SetImage(Image image) => ImageBase = (Bitmap)image;

        public void SetImage(string image) => ImageBase = (Bitmap)Image.FromFile(image);

        void ImageBaseChanged()
        {
            Sprite = new Sprite(ImageBase, '█', physicalState.Scale);
        }

        public void SetImage(Color[,] image) => colorBase = image;

        public void SetImage(Color[,] image, string printedChar)
        {
            colorBase = image;
            PrintedChar = printedChar;
        }

        void ColorBaseChanged()
        {
            Sprite = new Sprite(ColorBase, '█');
        }
    }
}
