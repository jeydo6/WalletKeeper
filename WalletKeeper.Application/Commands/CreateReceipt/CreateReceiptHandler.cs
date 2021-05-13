using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Security.Principal;
using System.Threading;
using System.Threading.Tasks;
using WalletKeeper.Application.Dto;
using WalletKeeper.Application.Extensions;
using WalletKeeper.Domain.Exceptions;
using WalletKeeper.Domain.Services;
using WalletKeeper.Domain.Types;
using WalletKeeper.Persistence.DbContexts;

namespace WalletKeeper.Application.Commands
{
	public class CreateReceiptHandler : IRequestHandler<CreateReceiptCommand, ReceiptDto>
	{
		private readonly ApplicationDbContext _dbContext;

		private readonly IPrincipal _principal;
		private readonly IFiscalDataService _fiscalDataService;
		private readonly ILogger<CreateReceiptHandler> _logger;

		public CreateReceiptHandler(
			ApplicationDbContext dbContext,
			IPrincipal principal,
			IFiscalDataService fiscalDataService,
			ILogger<CreateReceiptHandler> logger
		)
		{
			_dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
			_principal = principal ?? throw new ArgumentNullException(nameof(principal));
			_fiscalDataService = fiscalDataService ?? throw new ArgumentNullException(nameof(fiscalDataService));
			_logger = logger ?? throw new ArgumentNullException(nameof(logger));
		}

		public async Task<ReceiptDto> Handle(CreateReceiptCommand request, CancellationToken cancellationToken)
		{
			if (String.IsNullOrWhiteSpace(request.BarcodeString))
			{
				throw new ValidationException($"{nameof(request.BarcodeString)} is invalid");
			}

			if (!Guid.TryParse(_principal.GetUserID(), out var userID))
			{
				throw new BusinessException($"{nameof(userID)} is invalid");
			}

			var qrcode = QRCode.Parse(request.BarcodeString);

			var receipt = await _dbContext.Receipts.FirstOrDefaultAsync(r => r.FiscalDocumentNumber == qrcode.FiscalDocumentNumber, cancellationToken);
			if (receipt != null)
			{
				throw new BusinessException("Receipt already exists!");
			}

			receipt = await _fiscalDataService.GetReceipt(qrcode);
			receipt.UserID = userID;
			receipt.User = null;

			var organization = await _dbContext.Organizations.FirstOrDefaultAsync(o => o.INN == receipt.Organization.INN, cancellationToken);
			if (organization != null)
			{
				receipt.OrganizationID = organization.ID;
				receipt.Organization = null;
			}

			var productItems = await _dbContext.ProductItems.ToListAsync(cancellationToken);
			foreach (var item in receipt.ProductItems)
			{
				var productItem = productItems.FirstOrDefault(pi => pi.Name == item.Name && pi.ProductID != null);

				if (productItem != null)
				{
					item.ProductID = productItem.ProductID;
					item.Product = null;
				}
			}

			await _dbContext.Receipts.AddAsync(receipt, cancellationToken);
			await _dbContext.SaveChangesAsync(cancellationToken);

			var result = new ReceiptDto
			{
				FiscalDocumentNumber = receipt.FiscalDocumentNumber,
				FiscalDriveNumber = receipt.FiscalDriveNumber,
				FiscalType = receipt.FiscalType,
				DateTime = receipt.DateTime,
				OperationType = receipt.OperationType,
				TotalSum = receipt.TotalSum
			};

			return result;

		}
	}
}
