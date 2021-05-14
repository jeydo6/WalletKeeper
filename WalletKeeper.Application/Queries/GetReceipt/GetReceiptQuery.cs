using MediatR;
using System;
using WalletKeeper.Application.Dto;

namespace WalletKeeper.Application.Queries
{
	public class GetReceiptQuery : IRequest<ReceiptDto>
	{
		public GetReceiptQuery(Int32 id)
		{
			ID = id;
		}

		public Int32 ID { get; }
	}
}
}
