using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Globalization;
using System.Threading.Tasks;
using WalletKeeper.Application.Dto;
using WalletKeeper.Barcodes.Decoders;
using WalletKeeper.Domain.Entities;
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
		[Produces(typeof(ReceiptHeaderDto))]
		public async Task<IActionResult> CreateReceipt(GenericDto<Byte[]> receiptPhoto)
		{
			var barcodeString = _barcodeDecoder.Decode(receiptPhoto.Value);
			var receiptHeader = Parse(barcodeString);

			var receipt = await _dbContext.Receipts.FirstOrDefaultAsync(r => r.FiscalDocumentNumber == receiptHeader.FiscalDocumentNumber);
			if (receipt != null)
			{
				return BadRequest("Receipt already exists!");
			}

			receipt = await _fiscalDataService.GetReceipt(receiptHeader);

			var organization = await _dbContext.Organizations.FirstOrDefaultAsync(o => o.INN == receipt.Organization.INN);
			if (organization != null)
			{
				receipt.Organization = organization;
			}

			await _dbContext.Receipts.AddAsync(receipt);
			await _dbContext.SaveChangesAsync();

			var result = new ReceiptHeaderDto
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

		private static Receipt Parse(String barcodeString)
		{
			if (String.IsNullOrEmpty(barcodeString))
			{
				throw new ArgumentNullException(barcodeString);
			}

			if (!barcodeString.Contains("&"))
			{
				throw new ArgumentException(barcodeString);
			}

			String fiscalDocumentNumber = default;
			String fiscalDriveNumber = default;
			String fiscalType = default;
			DateTime dateTime = default;
			Decimal totalSum = default;
			Int32 operationType = default;

			foreach (var item in barcodeString.Split('&', StringSplitOptions.RemoveEmptyEntries))
			{
				if (item.StartsWith("i="))
				{
					fiscalDocumentNumber = item[2..];
				}
				else if (item.StartsWith("fn="))
				{
					fiscalDriveNumber = item[3..];
				}
				else if (item.StartsWith("fp="))
				{
					fiscalType = item[3..];
				}
				else if (item.StartsWith("t="))
				{
					dateTime = DateTime.ParseExact(item[2..], "yyyyMMddTHHmm", CultureInfo.InvariantCulture);
				}
				else if (item.StartsWith("s="))
				{
					totalSum = Decimal.Parse(item[2..], CultureInfo.InvariantCulture);
				}
				else if (item.StartsWith("n="))
				{
					operationType = Int32.Parse(item[2..]);
				}
			}

			return new Receipt
			{
				FiscalDocumentNumber = fiscalDocumentNumber,
				FiscalDriveNumber = fiscalDriveNumber,
				FiscalType = fiscalType,
				DateTime = dateTime,
				TotalSum = totalSum,
				OperationType = operationType
			};
		}
	}
}
