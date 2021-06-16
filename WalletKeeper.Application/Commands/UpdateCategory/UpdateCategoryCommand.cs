using MediatR;
using WalletKeeper.Application.Dto;

namespace WalletKeeper.Application.Commands
{
	public class UpdateCategoryCommand : IRequest<CategoryDto>
	{
		public UpdateCategoryCommand(CategoryDto dto)
		{
			Dto = dto;
		}

		public CategoryDto Dto { get; }
	}
}
