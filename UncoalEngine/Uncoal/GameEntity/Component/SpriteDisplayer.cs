using System.Drawing;

namespace Uncoal.Engine
{
	public class SpriteDisplayer : Component
	{
		public bool IsVisible = true;
		public int Layer;

		internal int generation = 0;

		public string[,] ColorMap => Sprite.spriteMap;

		public Sprite Sprite;

		public int Width => Sprite.spriteMap.GetLength(0);

		public int Heigth => Sprite.spriteMap.GetLength(1);

		public bool IsInitialized => Sprite?.spriteMap != null;

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
			generation++;
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
			generation++;
			Sprite = new Sprite(imageBaseString);
		}

		public void SetImage(Bitmap image) => ImageBase = image;

		public void SetImage(Image image) => ImageBase = (Bitmap)image;

		public void SetImage(string image) => ImageBase = (Bitmap)Image.FromFile(image);

		public void SetImage(string[,] image) => ImageBaseString = image;

		internal void RecalculateSpriteSize()
		{
			generation++;
			Sprite = new Sprite(imageBase, physicalState.Scale);
		}
	}
}
