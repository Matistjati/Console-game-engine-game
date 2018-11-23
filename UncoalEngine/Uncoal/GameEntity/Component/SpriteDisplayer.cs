using System.Drawing;

namespace Uncoal.Engine
{
	public class SpriteDisplayer : Component
	{
		public bool IsVisible = true;
		public int Layer;


		public string[,] ColorMap => Sprite.colorValues;

		internal Sprite Sprite { get; set; }

		public int Width => Sprite.Size.X;

		public int Heigth => Sprite.Size.Y;

		public bool IsInitialized => Sprite?.colorValues != null;

		private float lastScaleImage;
		Bitmap imageBase;

		public Bitmap ImageBase
		{
			get => imageBase;
			set
			{
				imageBase = value;
				if (lastScaleImage != physicalState.Scale)
				{
					ImageBaseChanged();
					lastScaleImage = physicalState.Scale;
				}
			}
		}

		void ImageBaseChanged()
		{
			Sprite = new Sprite(ImageBase, physicalState.Scale);
		}

		private string[,] imageBaseString;

		internal string[,] ImageBaseString
		{
			get => imageBaseString;
			set
			{
				imageBaseString = value;
				ImageBaseStringChanged();
			}
		}

		void ImageBaseStringChanged()
		{
			Sprite = new Sprite(imageBaseString);
		}

		public void SetImage(Bitmap image) => ImageBase = image;

		public void SetImage(Image image) => ImageBase = (Bitmap)image;

		public void SetImage(string image) => ImageBase = (Bitmap)Image.FromFile(image);

		public void SetImage(string[,] image) => ImageBaseString = image;

		internal void RecalculateSpriteSize()
		{
			Sprite = new Sprite(imageBase, physicalState.Scale);
		}
	}
}
