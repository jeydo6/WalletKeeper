using MediatR;
using System;
using WalletKeeper.Application.Dto;

namespace WalletKeeper.Application.Queries
{
	public class GetProductQuery : IRequest<ProductDto>
	{
		public GetProductQuery(Int32 id)
		{
			ID = id;
		}

		public Int32 ID { get; }
	}
}
