using MediatR;
using System;
using WalletKeeper.Application.Dto;

namespace WalletKeeper.Application.Queries
{
	public class StringifyReceiptQuery : IRequest<String>
	{
		public StringifyReceiptQuery(ReceiptDto dto)
		{
			Dto = dto;
		}

		public ReceiptDto Dto { get; }
	}
}