using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Security.Principal;
using System.Threading;
using System.Threading.Tasks;
using WalletKeeper.Domain.Entities;
using WalletKeeper.Domain.Exceptions;
using WalletKeeper.Domain.Extensions;
using WalletKeeper.Domain.Repositories;
using WalletKeeper.Persistence.DbContexts;

namespace WalletKeeper.Persistence.Repositories
{
	public class ProductItemsRepository : IProductItemsRepository
	{
		private readonly ApplicationDbContext _dbContext;

		private readonly IPrincipal _principal;
		private readonly ILogger<ProductItemsRepository> _logger;

		public ProductItemsRepository(
			ApplicationDbContext dbContext,
			IPrincipal principal,
			ILogger<ProductItemsRepository> logger
		)
		{
			_dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
			_principal = principal ?? throw new ArgumentNullException(nameof(principal));
			_logger = logger ?? throw new ArgumentNullException(nameof(logger));
		}

		public async Task<ProductItem[]> GetAsync(CancellationToken cancellationToken = default)
		{
			if (!Guid.TryParse(_principal.GetUserID(), out var userID))
			{
				throw new BusinessException($"{nameof(userID)} is invalid");
			}

			var productItems = await _dbContext.ProductItems
				.Where(pi => pi.Receipt.UserID == userID)
				.ToArrayAsync(cancellationToken);

			return productItems;
		}

		public async Task<ProductItem> GetAsync(Int32 id, CancellationToken cancellationToken = default)
		{
			if (!Guid.TryParse(_principal.GetUserID(), out var userID))
			{
				throw new BusinessException($"{nameof(userID)} is invalid");
			}

			var productItem = await _dbContext.ProductItems.FirstOrDefaultAsync(pi => pi.ID == id && pi.Receipt.UserID == userID, cancellationToken);

			return productItem;
		}

		public async Task<ProductItem> UpdateAsync(ProductItem item, CancellationToken cancellationToken = default)
		{
			if (!Guid.TryParse(_principal.GetUserID(), out var userID))
			{
				throw new BusinessException($"{nameof(userID)} is invalid");
			}

			var productItem = await _dbContext.ProductItems.FirstOrDefaultAsync(pi => pi.ID == item.ID && pi.Receipt.UserID == userID, cancellationToken);
			if (productItem == null)
			{
				throw new BusinessException("ProductItem is not exists!");
			}

			productItem.Name = item.Name;
			productItem.ProductID = item.ProductID;
			productItem.Product = null;

			await _dbContext.SaveChangesAsync(cancellationToken);

			return productItem;
		}
	}
}
