using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Threading.Tasks;
using WalletKeeper.Application.Dto;
using WalletKeeper.Barcodes.Decoders;
using WalletKeeper.Barcodes.Types;
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
		public async Task<IActionResult> CreateReceiptByPhoto(GenericDto<Byte[]> dto)
		{
			var barcodeString = _barcodeDecoder.Decode(dto.Value);
			var qrcode = QRCode.Parse(barcodeString);

			var receiptEntity = await _dbContext.Receipts.FirstOrDefaultAsync(r => r.FiscalDocumentNumber == qrcode.FiscalDocumentNumber);
			if (receiptEntity != null)
			{
				return BadRequest("Receipt already exists!");
			}

			var receipt = await _fiscalDataService.GetReceipt(qrcode);

			var organizationEntity = await _dbContext.Organizations.FirstOrDefaultAsync(o => o.INN == receipt.Organization.INN);
			if (organizationEntity != null)
			{
				receipt.OrganizationID = organizationEntity.ID;
				receipt.Organization = null;
			}

			var productItemEntities = await _dbContext.ProductItems.ToListAsync();
			foreach (var productItem in receipt.ProductItems)
			{
				var productItemEntity = productItemEntities.FirstOrDefault(pi => pi.Name == productItem.Name && pi.ProductID != null);

				if (productItemEntity != null)
				{
					productItem.ProductID = productItemEntity.ProductID;
					productItem.Product = null;
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

			return Ok(result);
		}

		[HttpPost("barcode")]
		[Produces(typeof(ReceiptDto))]
		public async Task<IActionResult> CreateReceiptByBarcode(GenericDto<String> dto)
		{
			var barcodeString = dto.Value;
			var qrcode = QRCode.Parse(barcodeString);

			var receiptEntity = await _dbContext.Receipts.FirstOrDefaultAsync(r => r.FiscalDocumentNumber == qrcode.FiscalDocumentNumber);
			if (receiptEntity != null)
			{
				return BadRequest("Receipt already exists!");
			}

			var receipt = await _fiscalDataService.GetReceipt(qrcode);

			var organizationEntity = await _dbContext.Organizations.FirstOrDefaultAsync(o => o.INN == receipt.Organization.INN);
			if (organizationEntity != null)
			{
				receipt.OrganizationID = organizationEntity.ID;
				receipt.Organization = null;
			}

			var productItemEntities = await _dbContext.ProductItems.ToListAsync();
			foreach (var productItem in receipt.ProductItems)
			{
				var productItemEntity = productItemEntities.FirstOrDefault(pi => pi.Name == productItem.Name && pi.ProductID != null);

				if (productItemEntity != null)
				{
					productItem.ProductID = productItemEntity.ProductID;
					productItem.Product = null;
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

			return Ok(result);
		}
	}
}
