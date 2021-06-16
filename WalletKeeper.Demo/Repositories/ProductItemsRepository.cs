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
	public class ProductItemsRepository : IProductItemsRepository
	{
		private readonly ApplicationDataContext _dataContext;

		private readonly ILogger<ProductItemsRepository> _logger;

		public ProductItemsRepository(
			ApplicationDataContext dataContext,
			ILogger<ProductItemsRepository> logger
		)
		{
			_dataContext = dataContext ?? throw new ArgumentNullException(nameof(dataContext));
			_logger = logger ?? throw new ArgumentNullException(nameof(logger));
		}

		public async Task<ProductItem[]> GetAsync(Guid userID, CancellationToken cancellationToken = default)
		{
			var productItems = _dataContext.ProductItems
				.Where(pi => pi.Receipt.UserID == userID)
				.ToArray();

			return await Task.FromResult(productItems);
		}

		public async Task<ProductItem> GetAsync(Int32 id, Guid userID, CancellationToken cancellationToken = default)
		{
			var productItem = _dataContext.ProductItems.FirstOrDefault(pi => pi.ID == id && pi.UserID == userID);

			return await Task.FromResult(productItem);
		}

		public async Task<ProductItem> UpdateAsync(ProductItem item, CancellationToken cancellationToken = default)
		{
			var productItem = _dataContext.ProductItems.FirstOrDefault(pi => pi.ID == item.ID && pi.UserID == item.UserID);
			if (productItem == null)
			{
				throw new BusinessException("ProductItem is not exists!");
			}

			productItem.Name = item.Name;

			var product = _dataContext.Products.FirstOrDefault(p => p.ID == item.ProductID);
			if (product != null)
			{
				productItem.ProductID = product.ID;
				productItem.Product = product;
			}

			return await Task.FromResult(productItem);
		}
	}
}
