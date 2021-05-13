using MediatR;
using WalletKeeper.Application.Dto;

namespace WalletKeeper.Application.Commands
{
	public class CreateCategoryCommand : IRequest<CategoryDto>
	{
		public CreateCategoryCommand(CategoryDto dto)
		{
			Dto = dto;
		}

		public CategoryDto Dto { get; }
	}
}
