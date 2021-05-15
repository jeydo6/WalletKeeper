using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Security.Principal;
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

		private readonly IPrincipal _principal;
		private readonly ILogger<ReceiptsRepository> _logger;

		public ReceiptsRepository(
			ApplicationDbContext dbContext,
			IPrincipal principal,
			ILogger<ReceiptsRepository> logger
		)
		{
			_dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
			_principal = principal ?? throw new ArgumentNullException(nameof(principal));
			_logger = logger ?? throw new ArgumentNullException(nameof(logger));
		}

		public async Task<Receipt[]> GetAsync(Guid userID, CancellationToken cancellationToken = default)
		{
			var receipts = await _dbContext.Receipts
				.Where(pi => pi.UserID == userID)
				.ToArrayAsync(cancellationToken);

			return receipts;
		}

		public async Task<Receipt> GetAsync(Int32 id, Guid userID, CancellationToken cancellationToken = default)
		{
			var receipt = await _dbContext.Receipts.FirstOrDefaultAsync(pi => pi.ID == id && pi.UserID == userID, cancellationToken);

			return receipt;
		}

		public async Task<Receipt> FindAsync(String fiscalDocumentNumber, CancellationToken cancellationToken = default)
		{
			var receipt = await _dbContext.Receipts.FirstOrDefaultAsync(r => r.FiscalDocumentNumber == fiscalDocumentNumber, cancellationToken);

			return receipt;
		}

		public async Task<Receipt> CreateAsync(Receipt item, Guid userID, CancellationToken cancellationToken = default)
		{
			item.UserID = userID;

			var productItems = await _dbContext.ProductItems.Where(pi => pi.ProductID != null).ToListAsync(cancellationToken);
			foreach (var productItem in item.ProductItems)
			{
				var temp = productItems.FirstOrDefault(pi => pi.Name == productItem.Name);
				if (temp != null)
				{
					productItem.ProductID = temp.ProductID;
				}
			}

			var organization = await _dbContext.Organizations.FirstOrDefaultAsync(o => o.INN == item.Organization.INN, cancellationToken);
			if (organization != null)
			{
				item.OrganizationID = organization.ID;
			}

			await _dbContext.Receipts.AddAsync(item, cancellationToken);
			await _dbContext.SaveChangesAsync(cancellationToken);

			return item;
		}

		public async Task DeleteAsync(Int32 id, Guid userID, CancellationToken cancellationToken = default)
		{
			var receipt = await _dbContext.Receipts.FirstOrDefaultAsync(pi => pi.ID == id && pi.UserID == userID, cancellationToken);
			if (receipt == null)
			{
				throw new BusinessException("ProductItem is not exists!");
			}

			_dbContext.Receipts.Remove(receipt);
			await _dbContext.SaveChangesAsync(cancellationToken);
		}
	}
}
