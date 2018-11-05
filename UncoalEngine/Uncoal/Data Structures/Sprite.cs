using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using Uncoal.Internal;

#if DEBUG
using System.Text;
#endif

namespace Uncoal.Engine
{
	public class Sprite
	{
		public readonly RGB[,] colorValues;
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
				image = ResizeImage(image, (int)(image.Width * scale), (int)(image.Width * scale));
			}

			Scale = scale;
			Size = new Coord(image.Width, image.Height);
			colorValues = new RGB[Size.X, Size.Y];

			for (int x = 0; x < Size.X; x++)
			{
				for (int y = 0; y < Size.Y; y++)
				{
					colorValues[x, y] = new RGB(image.GetPixel(x, y));
				}
			}

			if (scale != 1)
			{
				image.Dispose();
			}
		}

		public Sprite(RGB[,] image, char printedChar)
		{
			Size = new Coord(image.GetLength(0), image.GetLength(1));

			colorValues = image;
		}

#if DEBUG
		public override string ToString()
		{
			StringBuilder sprite = new StringBuilder(colorValues.GetLength(0) * colorValues.GetLength(1));
			for (int y = 0; y < colorValues.GetLength(1); y++)
			{
				for (int x = 0; x < colorValues.GetLength(0); x++)
				{
					RGB rgb = colorValues[x, y];
					if (rgb is null)
					{
						sprite.Append(" ");
					}
					else
					{
						sprite.Append(rgb.escapeSequence);
					}
				}
				sprite.Append('\n');
			}
			return sprite.ToString();
		}
#endif

		/// <summary>
		/// Resize the image to the specified width and height.
		/// </summary>
		/// <param name="image">The image to resize.</param>
		/// <param name="width">The width to resize to.</param>
		/// <param name="height">The height to resize to.</param>
		/// <returns>The resized image.</returns>
		public static Bitmap ResizeImage(Image image, int width, int height)
		{
			Rectangle destRect = new Rectangle(0, 0, width, height);
			Bitmap destImage = new Bitmap(width, height);

			destImage.SetResolution(image.HorizontalResolution, image.VerticalResolution);

			using (Graphics graphics = Graphics.FromImage(destImage))
			{
				// Most of these can priorotize high speed, as our images are small
				graphics.CompositingMode = CompositingMode.SourceCopy;
				graphics.CompositingQuality = CompositingQuality.HighSpeed;
				graphics.InterpolationMode = InterpolationMode.NearestNeighbor;
				graphics.SmoothingMode = SmoothingMode.HighSpeed;
				graphics.PixelOffsetMode = PixelOffsetMode.HighSpeed;

				using (ImageAttributes wrapMode = new ImageAttributes())
				{
					wrapMode.SetWrapMode(WrapMode.TileFlipXY);
					graphics.DrawImage(image, destRect, 0, 0, image.Width, image.Height, GraphicsUnit.Pixel, wrapMode);
				}
			}

			return destImage;
		}
	}
}
