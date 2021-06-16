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
	public class ReceiptsRepository : IReceiptsRepository
	{
		private readonly ApplicationDbContext _dbContext;

		private readonly ILogger<ReceiptsRepository> _logger;

		public ReceiptsRepository(
			ApplicationDbContext dbContext,
			ILogger<ReceiptsRepository> logger
		)
		{
			_dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
			_logger = logger ?? throw new ArgumentNullException(nameof(logger));
		}

		public async Task<Receipt[]> GetAsync(Guid userID, CancellationToken cancellationToken = default)
		{
			var receipts = await _dbContext.Receipts
				.Where(r => r.UserID == userID)
				.Include(r => r.ProductItems)
				.ToArrayAsync(cancellationToken);

			return receipts;
		}

		public async Task<Receipt> GetAsync(Int32 id, Guid userID, CancellationToken cancellationToken = default)
		{
			var receipt = await _dbContext.Receipts
				.Include(r => r.ProductItems)
				.FirstOrDefaultAsync(r => r.ID == id && r.UserID == userID, cancellationToken);

			return receipt;
		}

		public async Task<Receipt> FindAsync(String fiscalDocumentNumber, String fiscalDriveNumber, Guid userID, CancellationToken cancellationToken = default)
		{
			var receipt = await _dbContext.Receipts.FirstOrDefaultAsync(r =>
				r.FiscalDocumentNumber == fiscalDocumentNumber
				&& r.FiscalDriveNumber == fiscalDriveNumber
				&& r.UserID == userID,
				cancellationToken
			);

			return receipt;
		}

		public async Task<Receipt> CreateAsync(Receipt item, CancellationToken cancellationToken = default)
		{
			var products = await _dbContext.Products
				.Include(p => p.ProductItems)
				.OrderByDescending(p => p.ProductItems.Count)
				.ToListAsync(cancellationToken);
			foreach (var productItem in item.ProductItems)
			{
				var product = products.FirstOrDefault(p => p.ProductItems.Any(pi => pi.Name == productItem.Name));
				if (product != null)
				{
					if (product.UserID == item.UserID)
					{
						productItem.ProductID = product.ID;
						productItem.Product = null;
					}
					else
					{
						productItem.Product = new Product
						{
							Name = product.Name,
							CategoryID = product.CategoryID,
							UserID = item.UserID
						};
					}
				}
			}

			var organization = await _dbContext.Organizations.FirstOrDefaultAsync(o => o.INN == item.Organization.INN, cancellationToken);
			if (organization != null)
			{
				item.OrganizationID = organization.ID;
				item.Organization = null;
			}

			await _dbContext.Receipts.AddAsync(item, cancellationToken);
			await _dbContext.SaveChangesAsync(cancellationToken);

			return item;
		}

		public async Task DeleteAsync(Int32 id, Guid userID, CancellationToken cancellationToken = default)
		{
			var receipt = await _dbContext.Receipts
				.Include(r => r.ProductItems)
				.FirstOrDefaultAsync(r => r.ID == id && r.UserID == userID, cancellationToken);

			if (receipt == null)
			{
				throw new BusinessException("ProductItem is not exists!");
			}

			_dbContext.Receipts.Remove(receipt);
			_dbContext.ProductItems.RemoveRange(receipt.ProductItems);
			await _dbContext.SaveChangesAsync(cancellationToken);
		}
	}
}
