using MediatR;
using System;
using WalletKeeper.Application.Dto;

namespace WalletKeeper.Application.Queries
{
	public class GetCategoryQuery : IRequest<CategoryDto>
	{
		public GetCategoryQuery(Int32 id)
		{
			ID = id;
		}

		public Int32 ID { get; }
	}
}
