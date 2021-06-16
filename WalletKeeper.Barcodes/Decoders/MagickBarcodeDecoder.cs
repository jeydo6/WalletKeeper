using ImageMagick;
using System;
using WalletKeeper.Barcodes.Enumerations;
using WalletKeeper.Barcodes.Readers;
using ZXing;
using ZXing.Common;

namespace WalletKeeper.Barcodes.Decoders
{
	public abstract class MagickBarcodeDecoder : IBarcodeDecoder
	{
		private readonly IBarcodeReader<IMagickImage> _reader;
		private readonly Func<IMagickImage, Result>[] _methods;

		public MagickBarcodeDecoder(BarcodeFormatEnum barcodeFormat)
		{
			_methods = new Func<IMagickImage, Result>[]
			{
				AutoThresholdUndefined,
				AutoThresholdKapur,
				AutoThresholdOTSU,
				AutoThresholdTriangle,
				Quantize
			};
			_reader = new MagickBarcodeReader
			{
				Options = new DecodingOptions
				{
					PossibleFormats = new BarcodeFormat[]
					{
						(BarcodeFormat)barcodeFormat
					},
					TryHarder = true
				}
			};
		}

		public String Decode(IMagickImage barcodeImage)
		{
			return Decode(
				barcodeImage.ToByteArray()
			);
		}

		public String Decode(Byte[] barcodeImage)
		{
			foreach (var method in _methods)
			{
				var copy = new MagickImage(barcodeImage);
				var result = method(copy);
				if (result != null)
				{
					return result.Text;
				}
			}

			return null;
		}

		private Result Quantize(IMagickImage image)
		{
			var settings = new QuantizeSettings
			{
				ColorSpace = ColorSpace.Gray,
				Colors = 2,
				DitherMethod = DitherMethod.No
			};
			image.Quantize(settings);

			return _reader.Decode(image);
		}

		private Result AutoThresholdUndefined(IMagickImage image)
		{
			image.AutoThreshold(AutoThresholdMethod.Undefined);
			return _reader.Decode(image);
		}

		private Result AutoThresholdKapur(IMagickImage image)
		{
			image.AutoThreshold(AutoThresholdMethod.Kapur);
			return _reader.Decode(image);
		}

		private Result AutoThresholdOTSU(IMagickImage image)
		{
			image.AutoThreshold(AutoThresholdMethod.OTSU);
			return _reader.Decode(image);
		}

		private Result AutoThresholdTriangle(IMagickImage image)
		{
			image.AutoThreshold(AutoThresholdMethod.Triangle);
			return _reader.Decode(image);
		}
	}
}
