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
	public class ReceiptsRepository : IReceiptsRepository
	{
		private readonly ApplicationDataContext _dataContext;

		private readonly ILogger<ReceiptsRepository> _logger;

		public ReceiptsRepository(
			ApplicationDataContext dataContext,
			ILogger<ReceiptsRepository> logger
		)
		{
			_dataContext = dataContext ?? throw new ArgumentNullException(nameof(dataContext));
			_logger = logger ?? throw new ArgumentNullException(nameof(logger));
		}

		public async Task<Receipt[]> GetAsync(Guid userID, CancellationToken cancellationToken = default)
		{
			var receipts = _dataContext.Receipts
				.Where(pi => pi.UserID == userID)
				.ToArray();

			return await Task.FromResult(receipts);
		}

		public async Task<Receipt> GetAsync(Int32 id, Guid userID, CancellationToken cancellationToken = default)
		{
			var receipt = _dataContext.Receipts.FirstOrDefault(r => r.ID == id && r.UserID == userID);

			return await Task.FromResult(receipt);
		}

		public async Task<Receipt> FindAsync(String fiscalDriveNumber, CancellationToken cancellationToken = default)
		{
			var receipt = _dataContext.Receipts.FirstOrDefault(r => r.FiscalDriveNumber == fiscalDriveNumber);

			return await Task.FromResult(receipt);
		}

		public async Task<Receipt> CreateAsync(Receipt item, Guid userID, CancellationToken cancellationToken = default)
		{
			var user = _dataContext.Users.FirstOrDefault(u => u.Id == userID);
			if (user == null)
			{
				throw new BusinessException("User is not exists!");
			}

			item.UserID = userID;
			item.User = user;

			var productItems = _dataContext.ProductItems.Where(pi => pi.ProductID != null).ToList();
			foreach (var productItem in item.ProductItems)
			{
				var temp = productItems.FirstOrDefault(pi => pi.Name == productItem.Name);
				if (temp != null)
				{
					productItem.ProductID = temp.ProductID;
					productItem.Product = temp.Product;
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

		public async Task DeleteAsync(Int32 id, Guid userID, CancellationToken cancellationToken = default)
		{
			var receipt = _dataContext.Receipts.FirstOrDefault(r => r.ID == id && r.UserID == userID);

			if (receipt == null)
			{
				throw new BusinessException("ProductItem is not exists!");
			}

			_dataContext.Receipts.Remove(receipt);
			foreach (var productItem in receipt.ProductItems)
			{
				_dataContext.ProductItems.Remove(productItem);
			}

			await Task.CompletedTask;
		}
	}
}
