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
	public class GetProductHandler : IRequestHandler<GetProductQuery, ProductDto>
	{
		private readonly IProductsRepository _repository;
		private readonly ILogger<GetProductHandler> _logger;

		public GetProductHandler(
			IProductsRepository repository,
			ILogger<GetProductHandler> logger
		)
		{
			_repository = repository ?? throw new ArgumentNullException(nameof(repository));
			_logger = logger ?? throw new ArgumentNullException(nameof(logger));
		}

		public async Task<ProductDto> Handle(GetProductQuery request, CancellationToken cancellationToken)
		{
			if (request.ID <= 0)
			{
				throw new ValidationException($"{nameof(request.ID)} is invalid");
			}

			var product = await _repository.GetAsync(request.ID, cancellationToken);
			if (product == null)
			{
				throw new BusinessException("Product is not exists!");
			}

			var result = new ProductDto
			{
				ID = product.ID,
				Name = product.Name,
				CategoryID = product.CategoryID
			};

			return result;

		}
	}
}
