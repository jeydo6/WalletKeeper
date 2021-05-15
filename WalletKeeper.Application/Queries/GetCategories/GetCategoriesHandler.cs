using MediatR;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using WalletKeeper.Application.Dto;
using WalletKeeper.Domain.Repositories;

namespace WalletKeeper.Application.Queries
{
	public class GetCategoriesHandler : IRequestHandler<GetCategoriesQuery, CategoryDto[]>
	{
		private readonly ICategoriesRepository _repository;
		private readonly ILogger<GetCategoriesHandler> _logger;

		public GetCategoriesHandler(
			ICategoriesRepository repository,
			ILogger<GetCategoriesHandler> logger
		)
		{
			_repository = repository ?? throw new ArgumentNullException(nameof(repository));
			_logger = logger ?? throw new ArgumentNullException(nameof(logger));
		}

		public async Task<CategoryDto[]> Handle(GetCategoriesQuery request, CancellationToken cancellationToken)
		{
			var categories = await _repository.GetAsync(cancellationToken);

			var result = categories
				.Select(c => new CategoryDto
				{
					ID = c.ID,
					Name = c.Name
				})
				.ToArray();

			return result;
		}
	}
}
