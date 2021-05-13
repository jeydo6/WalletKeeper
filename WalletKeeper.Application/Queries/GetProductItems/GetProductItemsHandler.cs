using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Security.Claims;
using System.Security.Principal;
using System.Threading;
using System.Threading.Tasks;
using WalletKeeper.Application.Dto;
using WalletKeeper.Domain.Exceptions;
using WalletKeeper.Persistence.DbContexts;

namespace WalletKeeper.Application.Queries
{
	public class GetProductItemsHandler : IRequestHandler<GetProductItemsQuery, ProductItemDto[]>
	{
		private readonly ApplicationDbContext _dbContext;

		private readonly IPrincipal _principal;
		private readonly ILogger<GetProductItemsHandler> _logger;

		public GetProductItemsHandler(
			ApplicationDbContext dbContext,
			IPrincipal principal,
			ILogger<GetProductItemsHandler> logger
		)
		{
			_dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
			_principal = principal ?? throw new ArgumentNullException(nameof(principal));
			_logger = logger ?? throw new ArgumentNullException(nameof(logger));
		}

		public async Task<ProductItemDto[]> Handle(GetProductItemsQuery request, CancellationToken cancellationToken)
		{
			if (_principal is not ClaimsPrincipal claimsPrincipal)
			{
				throw new BusinessException($"{nameof(claimsPrincipal)} is invalid");
			}

			if (!Guid.TryParse(claimsPrincipal.FindFirstValue(ClaimTypes.NameIdentifier), out var userID))
			{
				throw new BusinessException($"{nameof(userID)} is invalid");
			}

			var result = await _dbContext.ProductItems
				.Where(pi => pi.Receipt.UserID == userID)
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
				.ToArrayAsync(cancellationToken);

			return result;
		}
	}
}
