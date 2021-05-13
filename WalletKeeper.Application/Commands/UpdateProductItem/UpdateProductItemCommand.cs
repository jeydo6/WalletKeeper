using MediatR;
using System;
using WalletKeeper.Application.Dto;

namespace WalletKeeper.Application.Commands
{
	public class UpdateProductItemCommand : IRequest<ProductItemDto>
	{
		public UpdateProductItemCommand(Int32 id, ProductItemDto dto)
		{
			ID = id;
			Dto = dto;
		}

		public Int32 ID { get; }

		public ProductItemDto Dto { get; }
	}
}
