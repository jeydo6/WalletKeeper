using System;

namespace WalletKeeper.Barcodes.Decoders
{
	public interface IBarcodeDecoder
	{
		String Decode(Byte[] image);
	}
}
