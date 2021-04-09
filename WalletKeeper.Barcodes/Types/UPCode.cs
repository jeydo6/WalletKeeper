using System;

namespace WalletKeeper.Barcodes.Types
{
	public class UPCode
	{
		private UPCode()
		{
			//
		}

		public Byte[] Values { get; private set; }

		public static UPCode Parse(String _)
		{
			return new UPCode
			{
				Values = new Byte[13]
			};
		}
	}
}
