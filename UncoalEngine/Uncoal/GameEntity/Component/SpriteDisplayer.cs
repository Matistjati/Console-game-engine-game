using System.Drawing;

namespace Uncoal.Engine
{
	public class SpriteDisplayer : Component
	{
		public bool IsVisible = true;

		public bool IsInitialized
		{
			get { return !(Sprite?.colorValues is null); }
		}

		public int Layer;

		internal void RecalculateSpriteSize()
		{
			Sprite = new Sprite(imageBase, physicalState.Scale);
		}

		internal Sprite Sprite { get; set; }

		public int Width { get => (int)Sprite.Size.X; }
		public int Heigth { get => (int)Sprite.Size.Y; }

		public string[,] ColorMap => Sprite.colorValues;

		private float lastScaleImage;

		Bitmap imageBase;

		internal Bitmap ImageBase
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


		private float lastScaleString;

		private string[,] imageBaseString;

		internal string[,] ImageBaseString
		{
			get => imageBaseString;
			set
			{
				imageBaseString = value;
				if (lastScaleString != physicalState.Scale)
				{
					ImageBaseStringChanged();
					lastScaleString = physicalState.Scale;
				}
			}
		}

		public void SetImage(Bitmap image) => ImageBase = image;

		public void SetImage(Image image) => ImageBase = (Bitmap)image;

		public void SetImage(string image) => ImageBase = (Bitmap)Image.FromFile(image);

		public void SetImage(string[,] image) => imageBaseString = image;




		void ImageBaseStringChanged()
		{
			Sprite = new Sprite(imageBaseString);
		}

		void ImageBaseChanged()
		{
			Sprite = new Sprite(ImageBase, physicalState.Scale);
		}
	}
}
