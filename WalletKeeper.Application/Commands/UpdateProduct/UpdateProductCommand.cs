using MediatR;
using WalletKeeper.Application.Dto;

namespace WalletKeeper.Application.Commands
{
	public class UpdateProductCommand : IRequest<ProductDto>
	{
		public UpdateProductCommand(ProductDto dto)
		{
			Dto = dto;
		}

		public ProductDto Dto { get; }
	}
}
