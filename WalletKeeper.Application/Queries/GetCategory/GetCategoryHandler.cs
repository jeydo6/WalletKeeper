using MediatR;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;
using WalletKeeper.Application.Dto;
using WalletKeeper.Domain.Exceptions;
using WalletKeeper.Domain.Repositories;

namespace WalletKeeper.Application.Queries
{
	public class GetCategoryHandler : IRequestHandler<GetCategoryQuery, CategoryDto>
	{
		private readonly ICategoriesRepository _repository;
		private readonly ILogger<GetCategoryHandler> _logger;

		public GetCategoryHandler(
			ICategoriesRepository repository,
			ILogger<GetCategoryHandler> logger
		)
		{
			_repository = repository ?? throw new ArgumentNullException(nameof(repository));
			_logger = logger ?? throw new ArgumentNullException(nameof(logger));
		}

		public async Task<CategoryDto> Handle(GetCategoryQuery request, CancellationToken cancellationToken)
		{
			if (request.ID <= 0)
			{
				throw new ValidationException($"{nameof(request.ID)} is invalid");
			}

			var category = await _repository.GetAsync(request.ID, cancellationToken);
			if (category == null)
			{
				throw new BusinessException("Category is not exists!");
			}

			var result = new CategoryDto
			{
				ID = category.ID,
				Name = category.Name
			};

			return result;
		}
	}
}
