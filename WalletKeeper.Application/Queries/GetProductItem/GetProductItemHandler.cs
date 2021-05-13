using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Security.Claims;
using System.Security.Principal;
using System.Threading;
using System.Threading.Tasks;
using WalletKeeper.Application.Dto;
using WalletKeeper.Domain.Exceptions;
using WalletKeeper.Persistence.DbContexts;

namespace WalletKeeper.Application.Queries
{
	public class GetProductItemHandler : IRequestHandler<GetProductItemQuery, ProductItemDto>
	{
		private readonly ApplicationDbContext _dbContext;

		private readonly IPrincipal _principal;
		private readonly ILogger<GetProductItemHandler> _logger;

		public GetProductItemHandler(
			ApplicationDbContext dbContext,
			IPrincipal principal,
			ILogger<GetProductItemHandler> logger
		)
		{
			_dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
			_principal = principal ?? throw new ArgumentNullException(nameof(principal));
			_logger = logger ?? throw new ArgumentNullException(nameof(logger));
		}

		public async Task<ProductItemDto> Handle(GetProductItemQuery request, CancellationToken cancellationToken)
		{
			if (request.ID <= 0)
			{
				throw new ValidationException($"{nameof(request.ID)} is invalid");
			}

			if (_principal is not ClaimsPrincipal claimsPrincipal)
			{
				throw new BusinessException($"{nameof(claimsPrincipal)} is invalid");
			}

			if (!Guid.TryParse(claimsPrincipal.FindFirstValue(ClaimTypes.NameIdentifier), out var userID))
			{
				throw new BusinessException($"{nameof(userID)} is invalid");
			}

			var productItem = await _dbContext.ProductItems.FirstOrDefaultAsync(pi => pi.ID == request.ID && pi.Receipt.UserID == userID, cancellationToken);
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
				ProductID = productItem.ProductID,
				ReceiptID = productItem.ReceiptID
			};

			return result;
		}
	}
}
