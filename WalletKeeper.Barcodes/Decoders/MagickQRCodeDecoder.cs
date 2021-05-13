using WalletKeeper.Barcodes.Enumerations;

namespace WalletKeeper.Barcodes.Decoders
{
	public class MagickQRCodeDecoder : MagickBarcodeDecoder
	{
		public MagickQRCodeDecoder() : base(BarcodeFormatEnum.QR_CODE)
		{
			//
		}
	}
}
