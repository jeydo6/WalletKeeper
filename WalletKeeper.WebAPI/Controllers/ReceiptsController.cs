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
using WalletKeeper.Barcodes.Types;
using WalletKeeper.Domain.Exceptions;
using WalletKeeper.Domain.Services;
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
			_dbContext = dbContext;
			_barcodeDecoder = barcodeDecoder;
			_fiscalDataService = fiscalDataService;
			_logger = logger;
		}

		[HttpPost("photo")]
		[Produces(typeof(ReceiptDto))]
		public async Task<IActionResult> PostPhoto(GenericDto<Byte[]> dto)
		{
			try
			{
				var userID = new Guid(User.FindFirstValue(ClaimTypes.NameIdentifier));
				var barcodeString = _barcodeDecoder.Decode(dto.Value);

				var result = await CreateReceipt(userID, barcodeString);

				return Ok(result);

			}
			catch (BusinessException ex)
			{
				return BadRequest(ex.Message);
			}
		}

		[HttpPost("barcode")]
		[Produces(typeof(ReceiptDto))]
		public async Task<IActionResult> PostBarcode(GenericDto<String> dto)
		{
			try
			{
				var userID = new Guid(User.FindFirstValue(ClaimTypes.NameIdentifier));
				var barcodeString = dto.Value;

				var result = await CreateReceipt(userID, barcodeString);

				return Ok(result);

			}
			catch (BusinessException ex)
			{
				return BadRequest(ex.Message);
			}
		}

		private async Task<ReceiptDto> CreateReceipt(Guid userID, String barcodeString)
		{
			if (userID == Guid.Empty)
			{
				throw new ArgumentNullException(nameof(userID));
			}

			if (String.IsNullOrWhiteSpace(barcodeString))
			{
				throw new ArgumentNullException(nameof(barcodeString));
			}

			var qrcode = QRCode.Parse(barcodeString);

			var receipt = await _dbContext.Receipts.FirstOrDefaultAsync(r => r.FiscalDocumentNumber == qrcode.FiscalDocumentNumber);
			if (receipt != null)
			{
				throw new BusinessException("Receipt already exists!");
			}

			receipt = await _fiscalDataService.GetReceipt(qrcode);
			receipt.User = null;
			receipt.UserID = userID;

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
