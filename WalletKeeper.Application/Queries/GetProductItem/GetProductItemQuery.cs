using MediatR;
using System;
using WalletKeeper.Application.Dto;

namespace WalletKeeper.Application.Queries
{
	public class GetProductItemQuery : IRequest<ProductItemDto>
	{
		public GetProductItemQuery(Int32 id)
		{
			ID = id;
		}

		public Int32 ID { get; }
	}
}
