using MediatR;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Security.Principal;
using System.Threading;
using System.Threading.Tasks;
using WalletKeeper.Application.Dto;
using WalletKeeper.Domain.Exceptions;
using WalletKeeper.Domain.Extensions;
using WalletKeeper.Domain.Repositories;

namespace WalletKeeper.Application.Queries
{
	public class GetProductItemsHandler : IRequestHandler<GetProductItemsQuery, ProductItemDto[]>
	{
		private readonly IPrincipal _principal;
		private readonly IProductItemsRepository _repository;
		private readonly ILogger<GetProductItemsHandler> _logger;

		public GetProductItemsHandler(
			IPrincipal principal,
			IProductItemsRepository repository,
			ILogger<GetProductItemsHandler> logger
		)
		{
			_principal = principal ?? throw new ArgumentNullException(nameof(principal));
			_repository = repository ?? throw new ArgumentNullException(nameof(repository));
			_logger = logger ?? throw new ArgumentNullException(nameof(logger));
		}

		public async Task<ProductItemDto[]> Handle(GetProductItemsQuery request, CancellationToken cancellationToken)
		{
			if (!Guid.TryParse(_principal.GetUserID(), out var userID))
			{
				throw new BusinessException($"{nameof(userID)} is invalid");
			}

			var productItems = await _repository.GetAsync(userID, cancellationToken);

			var result = productItems
				.Select(pi => new ProductItemDto
				{
					ID = pi.ID,
					Name = pi.Name,
					Price = pi.Price,
					Quantity = pi.Quantity,
					Sum = pi.Sum,
					ProductID = pi.ProductID,
					ReceiptID = pi.ReceiptID
				})
				.ToArray();

			return result;
		}
	}
}
