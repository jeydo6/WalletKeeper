using ImageMagick;
using System;
using WalletKeeper.Barcodes.Enumerations;
using WalletKeeper.Barcodes.Renderers;
using WalletKeeper.Barcodes.Writers;
using ZXing;
using ZXing.Common;

namespace WalletKeeper.Barcodes.Encoders
{
	public abstract class MagickBarcodeEncoder : IBarcodeEncoder
	{
		private readonly IBarcodeWriter<IMagickImage> _writer;

		public MagickBarcodeEncoder(BarcodeFormatEnum barcodeFormat)
		{
			_writer = new MagickBarcodeWriter()
			{
				Format = (BarcodeFormat)barcodeFormat,
				Options = new EncodingOptions
				{
					Width = 512,
					Height = 512,
					Margin = 1,
					PureBarcode = true
				},
				Renderer = new MagickImageRenderer()
			};
		}

		public Byte[] Encode(String barcodeString)
		{
			var result = _writer.Write(barcodeString);

			return result.ToByteArray(MagickFormat.Png);
		}
	}
}
