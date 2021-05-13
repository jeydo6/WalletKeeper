using MediatR;
using System;
using WalletKeeper.Application.Dto;

namespace WalletKeeper.Application.Commands
{
	public class UpdateCategoryCommand : IRequest<CategoryDto>
	{
		public UpdateCategoryCommand(Int32 id, CategoryDto dto)
		{
			ID = id;
			Dto = dto;
		}

		public Int32 ID { get; }

		public CategoryDto Dto { get; }
	}
}
