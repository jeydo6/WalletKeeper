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
	public class ProductsRepository : IProductsRepository
	{
		private readonly ApplicationDataContext _dataContext;

		private readonly IPrincipal _principal;
		private readonly ILogger<ProductsRepository> _logger;

		public ProductsRepository(
			ApplicationDataContext dataContext,
			IPrincipal principal,
			ILogger<ProductsRepository> logger
		)
		{
			_principal = principal ?? throw new ArgumentNullException(nameof(principal));
			_dataContext = dataContext ?? throw new ArgumentNullException(nameof(dataContext));
			_logger = logger ?? throw new ArgumentNullException(nameof(logger));
		}

		public async Task<Product[]> GetAsync(CancellationToken cancellationToken = default)
		{
			if (!Guid.TryParse(_principal.GetUserID(), out var userID))
			{
				throw new BusinessException($"{nameof(userID)} is invalid");
			}

			var products = _dataContext.Products
				.Where(p => p.UserID == userID)
				.ToArray();

			return await Task.FromResult(products);
		}

		public async Task<Product> GetAsync(Int32 id, CancellationToken cancellationToken = default)
		{
			if (!Guid.TryParse(_principal.GetUserID(), out var userID))
			{
				throw new BusinessException($"{nameof(userID)} is invalid");
			}

			var product = _dataContext.Products.FirstOrDefault(p => p.ID == id && p.UserID == userID);

			return await Task.FromResult(product);
		}

		public async Task<Product> CreateAsync(Product item, CancellationToken cancellationToken = default)
		{
			if (!Guid.TryParse(_principal.GetUserID(), out var userID))
			{
				throw new BusinessException($"{nameof(userID)} is invalid");
			}

			var product = _dataContext.Products.FirstOrDefault(p => p.Name == item.Name && p.UserID == userID);
			if (product != null)
			{
				throw new BusinessException("Product already exists!");
			}

			item.UserID = userID;
			item.User = _dataContext.Users.FirstOrDefault(u => u.Id == userID) ?? throw new BusinessException("User is not exists!");
			item.User.Products.Add(item);

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

		public async Task<Product> UpdateAsync(Product item, CancellationToken cancellationToken = default)
		{
			if (!Guid.TryParse(_principal.GetUserID(), out var userID))
			{
				throw new BusinessException($"{nameof(userID)} is invalid");
			}

			var product = _dataContext.Products.FirstOrDefault(p => p.ID == item.ID && p.UserID == userID);
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
			if (!Guid.TryParse(_principal.GetUserID(), out var userID))
			{
				throw new BusinessException($"{nameof(userID)} is invalid");
			}

			var product = _dataContext.Products.FirstOrDefault(p => p.ID == id && p.UserID == userID);
			if (product == null)
			{
				throw new BusinessException("Product is not exists!");
			}

			foreach (var productItem in product.ProductItems)
			{
				productItem.ProductID = null;
				productItem.Product = null;
			}

			product.User.Products.Remove(product);
			_dataContext.Products.Remove(product);

			await Task.CompletedTask;
		}
	}
}
