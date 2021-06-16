using MediatR;
using System;

namespace WalletKeeper.Application.Queries
{
	public class EncodeReceiptQuery : IRequest<Byte[]>
	{
		public EncodeReceiptQuery(String barcodeString)
		{
			BarcodeString = barcodeString;
		}

		public String BarcodeString { get; }
	}
}
