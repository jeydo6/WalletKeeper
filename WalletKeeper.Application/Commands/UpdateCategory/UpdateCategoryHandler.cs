using MediatR;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;
using WalletKeeper.Application.Dto;
using WalletKeeper.Domain.Entities;
using WalletKeeper.Domain.Exceptions;
using WalletKeeper.Domain.Repositories;

namespace WalletKeeper.Application.Commands
{
	public class UpdateCategoryHandler : IRequestHandler<UpdateCategoryCommand, CategoryDto>
	{
		private readonly ICategoriesRepository _repository;
		private readonly ILogger<UpdateCategoryHandler> _logger;

		public UpdateCategoryHandler(
			ICategoriesRepository repository,
			ILogger<UpdateCategoryHandler> logger
		)
		{
			_repository = repository ?? throw new ArgumentNullException(nameof(repository));
			_logger = logger ?? throw new ArgumentNullException(nameof(logger));
		}

		public async Task<CategoryDto> Handle(UpdateCategoryCommand request, CancellationToken cancellationToken)
		{
			if (request.Dto == null)
			{
				throw new ValidationException($"{nameof(request.Dto)} is invalid");
			}

			if (request.Dto.ID <= 0)
			{
				throw new ValidationException($"{nameof(request.Dto.ID)} is invalid");
			}

			var item = new Category
			{
				Name = request.Dto.Name
			};

			var category = await _repository.UpdateAsync(item, cancellationToken);

			var result = new CategoryDto
			{
				ID = category.ID,
				Name = category.Name
			};

			return result;
		}
	}
}
