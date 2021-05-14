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
using WalletKeeper.Persistence.Extensions;

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

		public async Task<Receipt[]> GetAsync(CancellationToken cancellationToken = default)
		{
			if (!Guid.TryParse(_principal.GetUserID(), out var userID))
			{
				throw new BusinessException($"{nameof(userID)} is invalid");
			}

			var receipts = await _dbContext.Receipts
				.Where(pi => pi.UserID == userID)
				.ToArrayAsync(cancellationToken);

			return receipts;
		}

		public async Task<Receipt> GetAsync(Int32 id, CancellationToken cancellationToken = default)
		{
			if (!Guid.TryParse(_principal.GetUserID(), out var userID))
			{
				throw new BusinessException($"{nameof(userID)} is invalid");
			}

			var receipt = await _dbContext.Receipts.FirstOrDefaultAsync(pi => pi.ID == id && pi.UserID == userID, cancellationToken);
			if (receipt == null)
			{
				throw new BusinessException("ProductItem is not exists!");
			}

			return receipt;
		}

		public async Task<Receipt> GetAsync(String fiscalDocumentNumber, CancellationToken cancellationToken = default)
		{
			var receipt = await _dbContext.Receipts.FirstOrDefaultAsync(r => r.FiscalDocumentNumber == fiscalDocumentNumber, cancellationToken);
			if (receipt != null)
			{
				throw new BusinessException("Receipt already exists!");
			}

			return receipt;
		}

		public async Task<Receipt> CreateAsync(Receipt item, CancellationToken cancellationToken = default)
		{
			if (!Guid.TryParse(_principal.GetUserID(), out var userID))
			{
				throw new BusinessException($"{nameof(userID)} is invalid");
			}

			var receipt = new Receipt
			{
				FiscalDocumentNumber = item.FiscalDocumentNumber,
				FiscalDriveNumber = item.FiscalDriveNumber,
				FiscalType = item.FiscalType,
				DateTime = item.DateTime,
				TotalSum = item.TotalSum,
				OperationType = item.OperationType,
				Place = item.Place,
				UserID = userID
			};

			var productItems = await _dbContext.ProductItems.ToListAsync(cancellationToken);
			receipt.ProductItems = item.ProductItems
				.Select(pi =>
				{
					var result = new ProductItem
					{
						Name = pi.Name,
						Price = pi.Price,
						Quantity = pi.Quantity,
						Sum = pi.Sum,
						Receipt = receipt
					};

					var productItem = productItems.FirstOrDefault(pi => pi.Name == result.Name);
					if (productItem != null)
					{
						result.ProductID = productItem.ProductID;
					}

					return result;
				})
				.ToList();

			var organization = await _dbContext.Organizations.FirstOrDefaultAsync(o => o.INN == item.Organization.INN, cancellationToken);
			if (organization != null)
			{
				receipt.OrganizationID = organization.ID;
			}
			else
			{
				receipt.Organization = new Organization
				{
					INN = item.Organization.INN,
					Name = item.Organization.Name
				};
			}

			await _dbContext.Receipts.AddAsync(receipt, cancellationToken);
			await _dbContext.SaveChangesAsync(cancellationToken);

			return receipt;
		}

		public async Task DeleteAsync(Int32 id, CancellationToken cancellationToken = default)
		{
			if (!Guid.TryParse(_principal.GetUserID(), out var userID))
			{
				throw new BusinessException($"{nameof(userID)} is invalid");
			}

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
