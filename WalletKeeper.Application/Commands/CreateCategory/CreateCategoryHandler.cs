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
	public class CreateCategoryHandler : IRequestHandler<CreateCategoryCommand, CategoryDto>
	{
		private readonly ICategoriesRepository _repository;
		private readonly ILogger<CreateCategoryHandler> _logger;

		public CreateCategoryHandler(
			ICategoriesRepository repository,
			ILogger<CreateCategoryHandler> logger
		)
		{
			_repository = repository ?? throw new ArgumentNullException(nameof(repository));
			_logger = logger ?? throw new ArgumentNullException(nameof(logger));
		}

		public async Task<CategoryDto> Handle(CreateCategoryCommand request, CancellationToken cancellationToken)
		{
			if (request.Dto == null)
			{
				throw new ValidationException($"{nameof(request.Dto)} is invalid");
			}

			var item = new Category
			{
				Name = request.Dto.Name
			};

			var category = await _repository.CreateAsync(item, cancellationToken);

			var result = new CategoryDto
			{
				ID = category.ID,
				Name = category.Name
			};

			return result;
		}
	}
}
