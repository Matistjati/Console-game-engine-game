using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Text;
using Uncoal.Internal;
using static Uncoal.Internal.NativeMethods;

namespace Uncoal.Engine
{
	public class Sprite
	{
		public float Scale;
		public CHAR_INFO[,] spriteMap;

		// Used for building colorValues
		const char blockChar = '█';

		public Sprite(CHAR_INFO[,] image)
		{
			spriteMap = image;
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


		public Sprite(Bitmap image, float scale)
		{
			if (scale <= 0)
			{
				throw new ArgumentOutOfRangeException(nameof(scale), scale, "Scale was less than or equal to 0");
			}

			if (scale != 1)
			{
				image = ResizeImage(image, (int)(image.Width * scale), (int)(image.Width * scale));
			}

			this.Scale = scale;

			this.spriteMap = new CHAR_INFO[image.Width, image.Height];


			unsafe
			{
				BitmapData bitmapData = image.LockBits(new Rectangle(0, 0, image.Width, image.Height), ImageLockMode.ReadOnly, image.PixelFormat);

				int bytesPerPixel = Image.GetPixelFormatSize(image.PixelFormat) / 8;
				int heightInPixels = bitmapData.Height;
				int widthInBytes = bitmapData.Width * bytesPerPixel;
				byte* ptrFirstPixel = (byte*)bitmapData.Scan0;

				for (int y = 0; y < heightInPixels; y++)
				{
					byte* currentLine = ptrFirstPixel + (y * bitmapData.Stride);
					for (int x = 0; x < widthInBytes; x += bytesPerPixel)
					{

						byte blue = currentLine[x];
						byte green = currentLine[x + 1];
						byte red = currentLine[x + 2];

						int xIndex = x / bytesPerPixel;

						if (blue == 0 && green == 0 && red == 0)
						{
							spriteMap[xIndex, y].UnicodeChar = ' ';
							spriteMap[xIndex, y].Attributes = 0;
						}
						else
						{
							spriteMap[xIndex, y].UnicodeChar = blockChar;
							spriteMap[xIndex, y].Attributes |= (CharAttribute)ConsoleColorHelper.ClosestConsoleColor(red, green, blue);
						}
					}
				}
				image.UnlockBits(bitmapData);
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
			StringBuilder sprite = new StringBuilder(spriteMap.GetLength(0) * spriteMap.GetLength(1));
			for (int y = 0; y < spriteMap.GetLength(1); y++)
			{
				for (int x = 0; x < spriteMap.GetLength(0); x++)
				{
					sprite.Append(spriteMap[x, y]);
				}
				sprite.Append('\n');
			}
			return sprite.ToString();
		}
#endif
	}
}
