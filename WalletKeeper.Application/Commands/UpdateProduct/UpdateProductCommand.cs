using MediatR;
using System;
using WalletKeeper.Application.Dto;

namespace WalletKeeper.Application.Commands
{
	public class UpdateProductCommand : IRequest<ProductDto>
	{
		public UpdateProductCommand(Int32 id, ProductDto dto)
		{
			ID = id;
			Dto = dto;
		}

		public Int32 ID { get; }

		public ProductDto Dto { get; }
	}
}
