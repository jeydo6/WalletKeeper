using ImageMagick;
using System;
using WalletKeeper.Barcodes.LuminianceSources;
using ZXing;

namespace WalletKeeper.Barcodes.Readers
{
	public class MagickBarcodeReader : BarcodeReader<IMagickImage>
	{
		/// <summary>
		/// define a custom function for creation of a luminance source with our specialized MagickImage-supporting class
		/// </summary>
		private static readonly Func<IMagickImage, LuminanceSource> _defaultCreateLuminanceSource = (image) => new MagickLuminanceSource(image);

		/// <summary>
		/// constructor which uses a custom luminance source with Mat support
		/// </summary>
		public MagickBarcodeReader()
			: base(null, _defaultCreateLuminanceSource, null)
		{
			//
		}
	}
}
