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
	public class GetProductItemHandler : IRequestHandler<GetProductItemQuery, ProductItemDto>
	{
		private readonly IProductItemsRepository _repository;
		private readonly ILogger<GetProductItemHandler> _logger;

		public GetProductItemHandler(
			IProductItemsRepository repository,
			ILogger<GetProductItemHandler> logger
		)
		{
			_repository = repository ?? throw new ArgumentNullException(nameof(repository));
			_logger = logger ?? throw new ArgumentNullException(nameof(logger));
		}

		public async Task<ProductItemDto> Handle(GetProductItemQuery request, CancellationToken cancellationToken)
		{
			if (request.ID <= 0)
			{
				throw new ValidationException($"{nameof(request.ID)} is invalid");
			}

			var productItem = await _repository.GetAsync(request.ID, cancellationToken);

			var result = new ProductItemDto
			{
				ID = productItem.ID,
				Name = productItem.Name,
				Price = productItem.Price,
				Quantity = productItem.Quantity,
				Sum = productItem.Sum,
				ProductID = productItem.ProductID,
				ReceiptID = productItem.ReceiptID
			};

			return result;
		}
	}
}
