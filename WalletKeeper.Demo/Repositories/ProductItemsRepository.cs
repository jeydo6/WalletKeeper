using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Security.Principal;
using System.Threading;
using System.Threading.Tasks;
using WalletKeeper.Demo.DataContexts;
using WalletKeeper.Domain.Entities;
using WalletKeeper.Domain.Exceptions;
using WalletKeeper.Domain.Extensions;
using WalletKeeper.Domain.Repositories;

namespace WalletKeeper.Demo.Repositories
{
	public class ProductItemsRepository : IProductItemsRepository
	{
		private readonly ApplicationDataContext _dataContext;

		private readonly IPrincipal _principal;
		private readonly ILogger<ProductItemsRepository> _logger;

		public ProductItemsRepository(
			ApplicationDataContext dataContext,
			IPrincipal principal,
			ILogger<ProductItemsRepository> logger
		)
		{
			_dataContext = dataContext ?? throw new ArgumentNullException(nameof(dataContext));
			_principal = principal ?? throw new ArgumentNullException(nameof(principal));
			_logger = logger ?? throw new ArgumentNullException(nameof(logger));
		}

		public async Task<ProductItem[]> GetAsync(CancellationToken cancellationToken = default)
		{
			if (!Guid.TryParse(_principal.GetUserID(), out var userID))
			{
				throw new BusinessException($"{nameof(userID)} is invalid");
			}

			var productItems = _dataContext.ProductItems
				.Where(pi => pi.Receipt.UserID == userID)
				.ToArray();

			return await Task.FromResult(productItems);
		}

		public async Task<ProductItem> GetAsync(Int32 id, CancellationToken cancellationToken = default)
		{
			if (!Guid.TryParse(_principal.GetUserID(), out var userID))
			{
				throw new BusinessException($"{nameof(userID)} is invalid");
			}

			var productItem = _dataContext.ProductItems.FirstOrDefault(pi => pi.ID == id && pi.Receipt.UserID == userID);

			return await Task.FromResult(productItem);
		}

		public async Task<ProductItem> UpdateAsync(ProductItem item, CancellationToken cancellationToken = default)
		{
			if (!Guid.TryParse(_principal.GetUserID(), out var userID))
			{
				throw new BusinessException($"{nameof(userID)} is invalid");
			}

			var productItem = _dataContext.ProductItems.FirstOrDefault(pi => pi.ID == item.ID && pi.Receipt.UserID == userID);
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
