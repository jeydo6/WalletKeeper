using ImageMagick;
using System;
using ZXing;

namespace WalletKeeper.Barcodes.LuminianceSources
{
	public class MagickLuminanceSource : BaseLuminanceSource
	{
		/// <summary>
		/// initializing constructor
		/// </summary>
		/// <param name="image"></param>
		public MagickLuminanceSource(IMagickImage image)
			: base(CalculateLuminance(image), image.Width, image.Height)
		{
			//
		}

		/// <summary>
		/// internal constructor used by CreateLuminanceSource
		/// </summary>
		/// <param name="luminances"></param>
		/// <param name="width"></param>
		/// <param name="height"></param>
		protected MagickLuminanceSource(Byte[] luminances, Int32 width, Int32 height)
			: base(luminances, width, height)
		{
		}

		/// <summary>
		/// Should create a new luminance source with the right class type.
		/// The method is used in methods crop and rotate.
		/// </summary>
		/// <param name="luminances">The new luminances.</param>
		/// <param name="width">The width.</param>
		/// <param name="height">The height.</param>
		/// <returns></returns>
		protected override LuminanceSource CreateLuminanceSource(Byte[] luminances, Int32 width, Int32 height)
		{
			return new MagickLuminanceSource(luminances, width, height);
		}

		private static Byte[] CalculateLuminance(IMagickImage image)
		{
			if (image == null)
			{
				throw new ArgumentNullException(nameof(image));
			}
				
			if (image.BitDepth() < 8)
			{
				image.BitDepth(8);
			}

			return image.ToByteArray(MagickFormat.Gray);
		}
	}
}
