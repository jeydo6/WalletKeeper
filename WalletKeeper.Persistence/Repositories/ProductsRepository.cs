using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
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

		public async Task<Product[]> GetAsync(CancellationToken cancellationToken = default)
		{
			var products = await _dbContext.Products
				.ToArrayAsync(cancellationToken);

			return products;
		}

		public async Task<Product> GetAsync(Int32 id, CancellationToken cancellationToken = default)
		{
			var product = await _dbContext.Products.FindAsync(new Object[] { id }, cancellationToken);
			if (product == null)
			{
				throw new BusinessException("Product is not exists!");
			}

			return product;
		}

		public async Task<Product> CreateAsync(Product item, CancellationToken cancellationToken = default)
		{
			var product = await _dbContext.Products.FirstOrDefaultAsync(c => c.Name == item.Name, cancellationToken);
			if (product != null)
			{
				throw new BusinessException("Product already exists!");
			}

			product = new Product
			{
				Name = item.Name,
				CategoryID = item.CategoryID
			};

			await _dbContext.Products.AddAsync(product, cancellationToken);
			await _dbContext.SaveChangesAsync(cancellationToken);

			return product;
		}

		public async Task<Product> UpdateAsync(Int32 id, Product item, CancellationToken cancellationToken = default)
		{
			var product = await _dbContext.Products.FindAsync(new Object[] { id }, cancellationToken);
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

		public async Task DeleteAsync(Int32 id, CancellationToken cancellationToken = default)
		{
			var product = await _dbContext.Products.FindAsync(new Object[] { id }, cancellationToken);
			if (product == null)
			{
				throw new BusinessException("Product is not exists!");
			}

			_dbContext.Products.Remove(product);
			await _dbContext.SaveChangesAsync(cancellationToken);
		}
	}
}
