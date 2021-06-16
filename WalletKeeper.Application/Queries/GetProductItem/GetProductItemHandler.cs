using MediatR;
using Microsoft.Extensions.Logging;
using System;
using System.Security.Principal;
using System.Threading;
using System.Threading.Tasks;
using WalletKeeper.Application.Dto;
using WalletKeeper.Domain.Exceptions;
using WalletKeeper.Domain.Extensions;
using WalletKeeper.Domain.Repositories;

namespace WalletKeeper.Application.Queries
{
	public class GetProductItemHandler : IRequestHandler<GetProductItemQuery, ProductItemDto>
	{
		private readonly IPrincipal _principal;
		private readonly IProductItemsRepository _repository;
		private readonly ILogger<GetProductItemHandler> _logger;

		public GetProductItemHandler(
			IPrincipal principal,
			IProductItemsRepository repository,
			ILogger<GetProductItemHandler> logger
		)
		{
			_principal = principal ?? throw new ArgumentNullException(nameof(principal));
			_repository = repository ?? throw new ArgumentNullException(nameof(repository));
			_logger = logger ?? throw new ArgumentNullException(nameof(logger));
		}

		public async Task<ProductItemDto> Handle(GetProductItemQuery request, CancellationToken cancellationToken)
		{
			if (request.ID <= 0)
			{
				throw new ValidationException($"{nameof(request.ID)} is invalid");
			}

			if (!Guid.TryParse(_principal.GetUserID(), out var userID))
			{
				throw new BusinessException($"{nameof(userID)} is invalid");
			}

			var productItem = await _repository.GetAsync(request.ID, userID, cancellationToken);
			if (productItem == null)
			{
				throw new BusinessException("ProductItem is not exists!");
			}

			var result = new ProductItemDto
			{
				ID = productItem.ID,
				Name = productItem.Name,
				Price = productItem.Price,
				Quantity = productItem.Quantity,
				Sum = productItem.Sum,
				NDS = productItem.NDS,
				ProductID = productItem.ProductID,
				ReceiptID = productItem.ReceiptID
			};

			return result;
		}
	}
}
