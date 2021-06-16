using MediatR;
using System;
using WalletKeeper.Application.Dto;

namespace WalletKeeper.Application.Queries
{
	public class FetchReceiptQuery : IRequest<ReceiptDto>
	{
		public FetchReceiptQuery(String barcodeString)
		{
			BarcodeString = barcodeString;
		}

		public String BarcodeString { get; }
	}
}
