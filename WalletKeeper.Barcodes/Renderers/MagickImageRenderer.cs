using ImageMagick;
using System;
using ZXing;
using ZXing.Common;
using ZXing.Rendering;

namespace WalletKeeper.Barcodes.Renderers
{
	/// <summary>
	/// renderer class which generates a IMagickImage from a BitMatrix
	/// </summary>
	internal class MagickImageRenderer : IBarcodeRenderer<IMagickImage<Byte>>
	{
		private readonly MagickFactory magickFactory;

		/// <summary>
		/// default constructor
		/// </summary>
		public MagickImageRenderer()
			: this(null)
		{
			//
		}

		/// <summary>
		/// constructor, which can be used if a special implementation of IMagickFactory is need.
		/// </summary>
		/// <param name="magickFactory"></param>
		public MagickImageRenderer(IMagickFactory magickFactory)
		{
			this.magickFactory = magickFactory as MagickFactory ?? new MagickFactory();
		}

		/// <summary>
		/// renders the BitMatrix as MagickImage
		/// </summary>
		/// <param name="matrix"></param>
		/// <param name="format"></param>
		/// <param name="content"></param>
		/// <returns></returns>
		public IMagickImage<Byte> Render(BitMatrix matrix, BarcodeFormat format, String content)
		{
			return Render(matrix, format, content, new EncodingOptions());
		}

		/// <summary>
		/// renders the BitMatrix as MagickImage
		/// </summary>
		/// <param name="matrix"></param>
		/// <param name="format"></param>
		/// <param name="content"></param>
		/// <param name="options"></param>
		/// <returns></returns>
		public IMagickImage<Byte> Render(BitMatrix matrix, BarcodeFormat format, String content, EncodingOptions options)
		{
			var header = System.Text.Encoding.UTF8.GetBytes($"P4\n{matrix.Width} {matrix.Height}\n");

			var rowBytes = matrix.Width / 8;

			if ((matrix.Width % 8) != 0)
			{
				rowBytes++;
			}

			var totalBuffer = new Byte[header.Length + rowBytes * matrix.Height];

			header.CopyTo(totalBuffer, 0);

			var bufferOffset = header.Length;

			for (var y = 0; y < matrix.Height; y++)
			{
				for (var x = 0; x < matrix.Width; x++)
				{
					if (matrix[x, y])
					{
						totalBuffer[bufferOffset] |= (Byte)(((Byte)1) << 7 - (x % 8));
					}

					if (x % 8 == 7)
					{
						bufferOffset++;
					}
				}

				if ((matrix.Width % 8) != 0)
				{
					bufferOffset++;
				}
			}

			return magickFactory.Image.Create(totalBuffer);
		}
	}
}
