using WalletKeeper.Barcodes.Enumerations;

namespace WalletKeeper.Barcodes.Encoders
{
	public class MagickQRCodeEncoder : MagickBarcodeEncoder
	{
		public MagickQRCodeEncoder() : base(BarcodeFormatEnum.QR_CODE)
		{
			//
		}
	}
}
