using MediatR;
using WalletKeeper.Application.Dto;

namespace WalletKeeper.Application.Commands
{
	public class CreateProductCommand : IRequest<ProductDto>
	{
		public CreateProductCommand(ProductDto dto)
		{
			Dto = dto;
		}

		public ProductDto Dto { get; }
	}
}
