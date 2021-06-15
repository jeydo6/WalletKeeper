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
	public class ReceiptsRepository : IReceiptsRepository
	{
		private readonly ApplicationDataContext _dataContext;

		private readonly IPrincipal _principal;
		private readonly ILogger<ReceiptsRepository> _logger;

		public ReceiptsRepository(
			ApplicationDataContext dataContext,
			IPrincipal principal,
			ILogger<ReceiptsRepository> logger
		)
		{
			_dataContext = dataContext ?? throw new ArgumentNullException(nameof(dataContext));
			_principal = principal ?? throw new ArgumentNullException(nameof(principal));
			_logger = logger ?? throw new ArgumentNullException(nameof(logger));
		}

		public async Task<Receipt[]> GetAsync(CancellationToken cancellationToken = default)
		{
			if (!Guid.TryParse(_principal.GetUserID(), out var userID))
			{
				throw new BusinessException($"{nameof(userID)} is invalid");
			}

			var receipts = _dataContext.Receipts
				.Where(r => r.UserID == userID)
				.ToArray();

			return await Task.FromResult(receipts);
		}

		public async Task<Receipt> GetAsync(Int32 id, CancellationToken cancellationToken = default)
		{
			if (!Guid.TryParse(_principal.GetUserID(), out var userID))
			{
				throw new BusinessException($"{nameof(userID)} is invalid");
			}

			var receipt = _dataContext.Receipts.FirstOrDefault(r => r.ID == id && r.UserID == userID);

			return await Task.FromResult(receipt);
		}

		public async Task<Receipt> FindAsync(String fiscalDocumentNumber, String fiscalDriveNumber, CancellationToken cancellationToken = default)
		{
			if (!Guid.TryParse(_principal.GetUserID(), out var userID))
			{
				throw new BusinessException($"{nameof(userID)} is invalid");
			}

			var receipt = _dataContext.Receipts.FirstOrDefault(r =>
				r.FiscalDocumentNumber == fiscalDocumentNumber
				&& r.FiscalDriveNumber == fiscalDriveNumber
				&& r.UserID == userID
			);

			return await Task.FromResult(receipt);
		}

		public async Task<Receipt> CreateAsync(Receipt item, CancellationToken cancellationToken = default)
		{
			if (!Guid.TryParse(_principal.GetUserID(), out var userID))
			{
				throw new BusinessException($"{nameof(userID)} is invalid");
			}

			item.UserID = userID;
			item.User = _dataContext.Users.FirstOrDefault(u => u.Id == userID) ?? throw new BusinessException("User is not exists!");
			item.User.Receipts.Add(item);

			var products = _dataContext.Products
				.OrderByDescending(p => p.ProductItems.Count)
				.ToList();
			foreach (var productItem in item.ProductItems)
			{
				var product = products.FirstOrDefault(p => p.ProductItems.Any(pi => pi.Name == productItem.Name));
				if (product != null)
				{
					if (product.UserID == item.UserID)
					{
						productItem.ProductID = product.ID;
						productItem.Product = product;
					}
					else
					{
						productItem.Product = new Product
						{
							Name = product.Name,
							CategoryID = product.CategoryID,
							UserID = item.UserID,
						};
					}
					productItem.Product.ProductItems.Add(productItem);
				}
			}

			var organization = _dataContext.Organizations.FirstOrDefault(o => o.INN == item.Organization.INN);
			if (organization != null)
			{
				item.OrganizationID = organization.ID;
				item.Organization = organization;
			}

			item.ID = _dataContext.Receipts.Max(c => c.ID) + 1;
			foreach (var productItem in item.ProductItems)
			{
				productItem.ID = _dataContext.ProductItems.Max(pi => pi.ID) + 1;
				productItem.ReceiptID = item.ID;

				_dataContext.ProductItems.Add(productItem);
			}
			_dataContext.Receipts.Add(item);

			return await Task.FromResult(item);
		}

		public async Task DeleteAsync(Int32 id, CancellationToken cancellationToken = default)
		{
			if (!Guid.TryParse(_principal.GetUserID(), out var userID))
			{
				throw new BusinessException($"{nameof(userID)} is invalid");
			}

			var receipt = _dataContext.Receipts.FirstOrDefault(r => r.ID == id && r.UserID == userID);

			if (receipt == null)
			{
				throw new BusinessException("ProductItem is not exists!");
			}

			receipt.User.Receipts.Remove(receipt);
			_dataContext.Receipts.Remove(receipt);
			foreach (var productItem in receipt.ProductItems)
			{
				_dataContext.ProductItems.Remove(productItem);
			}

			await Task.CompletedTask;
		}
	}
}
