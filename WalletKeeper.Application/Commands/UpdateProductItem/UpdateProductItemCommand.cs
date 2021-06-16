using MediatR;
using WalletKeeper.Application.Dto;

namespace WalletKeeper.Application.Commands
{
	public class UpdateProductItemCommand : IRequest<ProductItemDto>
	{
		public UpdateProductItemCommand(ProductItemDto dto)
		{
			Dto = dto;
		}

		public ProductItemDto Dto { get; }
	}
}
