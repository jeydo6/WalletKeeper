using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using WalletKeeper.Demo.DataContexts;
using WalletKeeper.Domain.Entities;
using WalletKeeper.Domain.Exceptions;
using WalletKeeper.Domain.Repositories;

namespace WalletKeeper.Demo.Repositories
{
	public class ProductsRepository : IProductsRepository
	{
		private readonly ApplicationDataContext _dataContext;

		private readonly ILogger<ProductsRepository> _logger;

		public ProductsRepository(
			ApplicationDataContext dataContext,
			ILogger<ProductsRepository> logger
		)
		{
			_dataContext = dataContext ?? throw new ArgumentNullException(nameof(dataContext));
			_logger = logger ?? throw new ArgumentNullException(nameof(logger));
		}

		public async Task<Product[]> GetAsync(CancellationToken cancellationToken = default)
		{
			var products = _dataContext.Products.ToArray();

			return await Task.FromResult(products);
		}

		public async Task<Product> GetAsync(Int32 id, CancellationToken cancellationToken = default)
		{
			var product = _dataContext.Products.FirstOrDefault(c => c.ID == id);

			return await Task.FromResult(product);
		}

		public async Task<Product> CreateAsync(Product item, CancellationToken cancellationToken = default)
		{
			var product = _dataContext.Products.FirstOrDefault(c => c.Name == item.Name);
			if (product != null)
			{
				throw new BusinessException("Product already exists!");
			}

			var category = _dataContext.Categories.FirstOrDefault(c => c.ID == item.CategoryID);
			if (category != null)
			{
				item.CategoryID = category.ID;
				item.Category = category;
			}

			item.ID = _dataContext.Products.Max(c => c.ID) + 1;
			_dataContext.Products.Add(item);

			return await Task.FromResult(item);
		}

		public async Task<Product> UpdateAsync(Int32 id, Product item, CancellationToken cancellationToken = default)
		{
			var product = _dataContext.Products.FirstOrDefault(c => c.ID == id);
			if (product == null)
			{
				throw new BusinessException("Product is not exists!");
			}

			product.Name = item.Name;

			var category = _dataContext.Categories.FirstOrDefault(c => c.ID == item.CategoryID);
			if (category != null)
			{
				product.CategoryID = category.ID;
				product.Category = category;
			}

			return await Task.FromResult(product);
		}

		public async Task DeleteAsync(Int32 id, CancellationToken cancellationToken = default)
		{
			var product = _dataContext.Products.FirstOrDefault(p => p.ID == id);
			if (product == null)
			{
				throw new BusinessException("Product is not exists!");
			}

			foreach (var productItem in product.ProductItems)
			{
				productItem.ProductID = null;
				productItem.Product = null;
			}

			_dataContext.Products.Remove(product);

			await Task.CompletedTask;
		}
	}
}
