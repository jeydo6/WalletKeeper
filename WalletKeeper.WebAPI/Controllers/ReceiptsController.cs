using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
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

		[HttpPost]
		[Produces(typeof(ReceiptDto))]
		public async Task<IActionResult> CreateReceipt(GenericDto<Byte[]> receiptPhoto)
		{
			var barcodeString = _barcodeDecoder.Decode(receiptPhoto.Value);
			var qrcode = QRCode.Parse(barcodeString);

			var receipt = await _dbContext.Receipts.FirstOrDefaultAsync(r => r.FiscalDocumentNumber == qrcode.FiscalDocumentNumber);
			if (receipt != null)
			{
				return BadRequest("Receipt already exists!");
			}

			receipt = await _fiscalDataService.GetReceipt(qrcode);

			var organization = await _dbContext.Organizations.FirstOrDefaultAsync(o => o.INN == receipt.Organization.INN);
			if (organization != null)
			{
				receipt.Organization = organization;
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
