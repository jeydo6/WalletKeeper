using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using WalletKeeper.Application.Dto;
using WalletKeeper.Barcodes.Decoders;
using WalletKeeper.Domain.Exceptions;
using WalletKeeper.Domain.Services;
using WalletKeeper.Domain.Types;
using WalletKeeper.Persistence.DbContexts;

namespace WalletKeeper.WebAPI.Controllers
{
	[Authorize]
	[ApiController]
	[Route("receipts")]
	public class ReceiptsController : ControllerBase
	{
		private readonly ApplicationDbContext _dbContext;
		private readonly IBarcodeDecoder _barcodeDecoder;
		private readonly IFiscalDataService _fiscalDataService;
		private readonly ILogger<ReceiptsController> _logger;

		public ReceiptsController(
			ApplicationDbContext dbContext,
			IBarcodeDecoder barcodeDecoder,
			IFiscalDataService fiscalDataService,
			ILogger<ReceiptsController> logger
		)
		{
			_dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
			_barcodeDecoder = barcodeDecoder ?? throw new ArgumentNullException(nameof(barcodeDecoder));
			_fiscalDataService = fiscalDataService ?? throw new ArgumentNullException(nameof(fiscalDataService));
			_logger = logger ?? throw new ArgumentNullException(nameof(logger));
		}

		[HttpPost("photo")]
		[Produces(typeof(ReceiptDto))]
		public async Task<IActionResult> Post(GenericDto<Byte[]> dto)
		{
			var userIDString = User.FindFirstValue(ClaimTypes.NameIdentifier);
			if (String.IsNullOrWhiteSpace(userIDString))
			{
				throw new BusinessException("User is unauthorized!");
			}
			var userID = new Guid(userIDString);
			var barcodeString = _barcodeDecoder.Decode(dto.Value);

			var result = await CreateReceipt(userID, barcodeString);

			return Ok(result);
		}

		[HttpPost("barcode")]
		[Produces(typeof(ReceiptDto))]
		public async Task<IActionResult> Post(GenericDto<String> dto)
		{
			var userIDString = User.FindFirstValue(ClaimTypes.NameIdentifier);
			if (String.IsNullOrWhiteSpace(userIDString))
			{
				throw new BusinessException("User is unauthorized!");
			}
			var userID = new Guid(userIDString);
			var barcodeString = dto.Value;

			var result = await CreateReceipt(userID, barcodeString);

			return Ok(result);
		}

		private async Task<ReceiptDto> CreateReceipt(Guid userID, String barcodeString)
		{
			if (userID == Guid.Empty)
			{
				throw new ValidationException($"{nameof(userID)} is invalid");
			}

			if (String.IsNullOrWhiteSpace(barcodeString))
			{
				throw new ValidationException($"{nameof(barcodeString)} is invalid");
			}

			var qrcode = QRCode.Parse(barcodeString);

			var receipt = await _dbContext.Receipts.FirstOrDefaultAsync(r => r.FiscalDocumentNumber == qrcode.FiscalDocumentNumber);
			if (receipt != null)
			{
				throw new BusinessException("Receipt already exists!");
			}

			receipt = await _fiscalDataService.GetReceipt(qrcode);
			receipt.UserID = userID;
			receipt.User = null;

			var organization = await _dbContext.Organizations.FirstOrDefaultAsync(o => o.INN == receipt.Organization.INN);
			if (organization != null)
			{
				receipt.OrganizationID = organization.ID;
				receipt.Organization = null;
			}

			var productItems = await _dbContext.ProductItems.ToListAsync();
			foreach (var item in receipt.ProductItems)
			{
				var productItem = productItems.FirstOrDefault(pi => pi.Name == item.Name && pi.ProductID != null);

				if (productItem != null)
				{
					item.ProductID = productItem.ProductID;
					item.Product = null;
				}
			}

			await _dbContext.Receipts.AddAsync(receipt);
			await _dbContext.SaveChangesAsync();

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
