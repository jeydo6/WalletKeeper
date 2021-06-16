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
	public class ProductsRepository : IProductsRepository
	{
		private readonly ApplicationDbContext _dbContext;

		private readonly ILogger<ProductsRepository> _logger;

		public ProductsRepository(
			ApplicationDbContext dbContext,
			ILogger<ProductsRepository> logger
		)
		{
			_dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
			_logger = logger ?? throw new ArgumentNullException(nameof(logger));
		}

		public async Task<Product[]> GetAsync(Guid userID, CancellationToken cancellationToken = default)
		{
			var products = await _dbContext.Products
				.Where(p => p.UserID == userID)
				.ToArrayAsync(cancellationToken);

			return products;
		}

		public async Task<Product> GetAsync(Int32 id, Guid userID, CancellationToken cancellationToken = default)
		{
			var product = await _dbContext.Products.FirstOrDefaultAsync(p => p.ID == id && p.UserID == userID, cancellationToken);

			return product;
		}

		public async Task<Product> CreateAsync(Product item, CancellationToken cancellationToken = default)
		{
			var product = await _dbContext.Products.FirstOrDefaultAsync(p => p.Name == item.Name && p.UserID == item.UserID, cancellationToken);
			if (product != null)
			{
				throw new BusinessException("Product already exists!");
			}

			await _dbContext.Products.AddAsync(item, cancellationToken);
			await _dbContext.SaveChangesAsync(cancellationToken);

			return item;
		}

		public async Task<Product> UpdateAsync(Product item, CancellationToken cancellationToken = default)
		{
			var product = await _dbContext.Products.FirstOrDefaultAsync(p => p.ID == item.ID && p.UserID == item.UserID, cancellationToken);
			if (product == null)
			{
				throw new BusinessException("Product is not exists!");
			}

			product.Name = item.Name;
			product.CategoryID = item.CategoryID;
			product.Category = null;

			await _dbContext.SaveChangesAsync(cancellationToken);

			return product;
		}

		public async Task DeleteAsync(Int32 id, Guid userID, CancellationToken cancellationToken = default)
		{
			var product = await _dbContext.Products
				.Include(p => p.ProductItems)
				.FirstOrDefaultAsync(p => p.ID == id && p.UserID == userID, cancellationToken);

			if (product == null)
			{
				throw new BusinessException("Product is not exists!");
			}

			foreach (var productItem in product.ProductItems)
			{
				productItem.ProductID = null;
				productItem.Product = null;
			}

			_dbContext.Products.Remove(product);
			await _dbContext.SaveChangesAsync(cancellationToken);
		}
	}
}
