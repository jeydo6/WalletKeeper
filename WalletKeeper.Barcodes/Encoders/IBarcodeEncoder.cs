using System;

namespace WalletKeeper.Barcodes.Encoders
{
	public interface IBarcodeEncoder
	{
		Byte[] Encode(String barcodeString);
	}
}
