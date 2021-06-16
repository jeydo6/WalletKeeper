using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using WalletKeeper.Domain.Entities;
using WalletKeeper.Domain.Exceptions;
using WalletKeeper.Domain.Repositories;
using WalletKeeper.Persistence.DbContexts;

namespace WalletKeeper.Persistence.Repositories
{
	public class ProductItemsRepository : IProductItemsRepository
	{
		private readonly ApplicationDbContext _dbContext;

		private readonly ILogger<ProductItemsRepository> _logger;

		public ProductItemsRepository(
			ApplicationDbContext dbContext,
			ILogger<ProductItemsRepository> logger
		)
		{
			_dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
			_logger = logger ?? throw new ArgumentNullException(nameof(logger));
		}

		public async Task<ProductItem[]> GetAsync(Guid userID, CancellationToken cancellationToken = default)
		{
			var productItems = await _dbContext.ProductItems
				.Where(pi => pi.UserID == userID)
				.ToArrayAsync(cancellationToken);

			return productItems;
		}

		public async Task<ProductItem> GetAsync(Int32 id, Guid userID, CancellationToken cancellationToken = default)
		{
			var productItem = await _dbContext.ProductItems.FirstOrDefaultAsync(pi => pi.ID == id && pi.UserID == userID, cancellationToken);

			return productItem;
		}

		public async Task<ProductItem> UpdateAsync(ProductItem item, CancellationToken cancellationToken = default)
		{
			var productItem = await _dbContext.ProductItems.FirstOrDefaultAsync(pi => pi.ID == item.ID && pi.UserID == item.UserID, cancellationToken);
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
