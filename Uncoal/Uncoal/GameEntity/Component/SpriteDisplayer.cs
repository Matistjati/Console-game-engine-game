using System.Drawing;

namespace Uncoal.Engine
{
	class SpriteDisplayer : Component
	{
		public bool IsVisible { get; set; } = true;

		public bool IsInitialized
		{
			get { return !((Sprite?.colorValues ?? null) is null); }
		}

		public int Layer { get; set; }

		public void RecalculateSpriteSize()
		{
			Sprite = new Sprite(imageBase, physicalState.Scale);
		}

		internal Sprite Sprite { get; set; }

		// More overloads for setting pic
		// Max length 1
		// Unnecessary
		public string PrintedChar { get; set; } = "█";

		public int Width { get => (int)Sprite.Size.X; }
		public int Heigth { get => (int)Sprite.Size.Y; }

		public RGB[,] ColorMap
		{
			get => Sprite.colorValues;
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
			Sprite = new Sprite(ImageBase, physicalState.Scale);
		}
	}
}
