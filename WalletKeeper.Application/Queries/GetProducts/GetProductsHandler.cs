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
	public class GetProductsHandler : IRequestHandler<GetProductsQuery, ProductDto[]>
	{
		private readonly IProductsRepository _repository;
		private readonly ILogger<GetProductsHandler> _logger;

		public GetProductsHandler(
			IProductsRepository repository,
			ILogger<GetProductsHandler> logger
		)
		{
			_repository = repository ?? throw new ArgumentNullException(nameof(repository));
			_logger = logger ?? throw new ArgumentNullException(nameof(logger));
		}

		public async Task<ProductDto[]> Handle(GetProductsQuery request, CancellationToken cancellationToken)
		{
			var products = await _repository.GetAsync(cancellationToken);

			var result = products
				.Select(p => new ProductDto
				{
					ID = p.ID,
					Name = p.Name,
					CategoryID = p.CategoryID
				})
				.ToArray();

			return result;
		}
	}
}
