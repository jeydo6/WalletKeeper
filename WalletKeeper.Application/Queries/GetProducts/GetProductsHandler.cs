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
	public class GetProductsHandler : IRequestHandler<GetProductsQuery, ProductDto[]>
	{
		private readonly IPrincipal _principal;
		private readonly IProductsRepository _repository;
		private readonly ILogger<GetProductsHandler> _logger;

		public GetProductsHandler(
			IPrincipal principal,
			IProductsRepository repository,
			ILogger<GetProductsHandler> logger
		)
		{
			_principal = principal ?? throw new ArgumentNullException(nameof(principal));
			_repository = repository ?? throw new ArgumentNullException(nameof(repository));
			_logger = logger ?? throw new ArgumentNullException(nameof(logger));
		}

		public async Task<ProductDto[]> Handle(GetProductsQuery request, CancellationToken cancellationToken)
		{
			if (!Guid.TryParse(_principal.GetUserID(), out var userID))
			{
				throw new BusinessException($"{nameof(userID)} is invalid");
			}

			var products = await _repository.GetAsync(userID, cancellationToken);

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
