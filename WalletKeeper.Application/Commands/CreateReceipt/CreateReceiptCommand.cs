using MediatR;
using System;
using WalletKeeper.Application.Dto;

namespace WalletKeeper.Application.Commands
{
	public class CreateReceiptCommand : IRequest<ReceiptDto>
	{
		public CreateReceiptCommand(String barcodeString)
		{
			BarcodeString = barcodeString;
		}

		public String BarcodeString { get; }
	}
}
