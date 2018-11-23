using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Text;

namespace Uncoal.Engine
{
	public class Sprite
	{
		public readonly float Scale;
		public readonly string[,] colorValues;
		public readonly Coord Size;

		// Used for building colorValues
		const string whiteSpace = " ";
		const string escapeStartRGB = "\x1b[38;2;";
		const string escapeEnd = "m█";
		const char colorSeparator = ';';

		public Sprite(string[,] image)
		{
			Size = new Coord(image.GetLength(0), image.GetLength(1));
			colorValues = image;
		}

		public Sprite(string image) : this((Bitmap)Image.FromFile(image), 1f)
		{ }

		public Sprite(string image, float scale) : this((Bitmap)Image.FromFile(image), scale)
		{ }

		public Sprite(Image image) : this(new Bitmap(image), 1f)
		{ }

		public Sprite(Image image, float scale) : this(new Bitmap(image), scale)
		{ }

		public Sprite(Bitmap image) : this(image, 1f)
		{ }

		static StringBuilder colorStringBuilder = new StringBuilder(24);

		public Sprite(Bitmap image, float scale)
		{
			if (scale != 1)
			{
				image = ResizeImage(image, (int)(image.Width * scale), (int)(image.Width * scale));
			}

			Scale = scale;
			Size = new Coord(image.Width, image.Height);
			colorValues = new string[Size.X, Size.Y];

			for (int x = 0; x < Size.X; x++)
			{
				for (int y = 0; y < Size.Y; y++)
				{
					Color rgb = image.GetPixel(x, y);

					if (rgb.R == 0 && rgb.G == 0 && rgb.B == 0) //|| rgb.A < 10)
					{
						colorValues[x, y] = whiteSpace;
					}
					else
					{
						// I know, it looks messy but it's the fastest way
						// An escape sequence telling the console what color to display
						// For more info, check
						// https://docs.microsoft.com/en-us/windows/console/console-virtual-terminal-sequences#extended-colors

						colorStringBuilder.Append(escapeStartRGB);
						colorStringBuilder.Append(rgb.R);
						colorStringBuilder.Append(colorSeparator);
						colorStringBuilder.Append(rgb.G);
						colorStringBuilder.Append(colorSeparator);
						colorStringBuilder.Append(rgb.B);
						colorStringBuilder.Append(escapeEnd);

						colorValues[x, y] = colorStringBuilder.ToString();
						colorStringBuilder.Clear();
					}
				}
			}

			image.Dispose();
		}

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

#if DEBUG
		public override string ToString()
		{
			StringBuilder sprite = new StringBuilder(colorValues.GetLength(0) * colorValues.GetLength(1));
			for (int y = 0; y < colorValues.GetLength(1); y++)
			{
				for (int x = 0; x < colorValues.GetLength(0); x++)
				{
					sprite.Append(colorValues[x, y]);
				}
				sprite.Append('\n');
			}
			return sprite.ToString();
		}
#endif
	}
}
