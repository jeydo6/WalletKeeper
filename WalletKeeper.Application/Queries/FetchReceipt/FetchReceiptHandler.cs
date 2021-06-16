using MediatR;
using Microsoft.Extensions.Logging;
using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using WalletKeeper.Application.Dto;
using WalletKeeper.Domain.Services;
using WalletKeeper.Domain.Types;

namespace WalletKeeper.Application.Queries
{
	public class FetchReceiptHandler : IRequestHandler<FetchReceiptQuery, ReceiptDto>
	{
		private readonly IFiscalDataService _fiscalDataService;
		private readonly ILogger<FetchReceiptHandler> _logger;

		public FetchReceiptHandler(
			IFiscalDataService fiscalDataService,
			ILogger<FetchReceiptHandler> logger
		)
		{
			_fiscalDataService = fiscalDataService ?? throw new ArgumentNullException(nameof(fiscalDataService));
			_logger = logger ?? throw new ArgumentNullException(nameof(logger));
		}

		public async Task<ReceiptDto> Handle(FetchReceiptQuery request, CancellationToken cancellationToken)
		{
			if (String.IsNullOrWhiteSpace(request.BarcodeString))
			{
				throw new ValidationException($"{nameof(request.BarcodeString)} is invalid");
			}

			var qrcode = QRCode.Parse(request.BarcodeString);

			var receipt = await _fiscalDataService.GetReceipt(qrcode);

			var result = new ReceiptDto
			{
				ID = receipt.ID,
				FiscalDocumentNumber = receipt.FiscalDocumentNumber,
				FiscalDriveNumber = receipt.FiscalDriveNumber,
				FiscalType = receipt.FiscalType,
				DateTime = receipt.DateTime,
				TotalSum = receipt.TotalSum,
				OperationType = receipt.OperationType,
				Place = receipt.Place,
				Organization = new OrganizationDto
				{
					INN = receipt.Organization.INN,
					Name = receipt.Organization.Name
				},
				ProductItems = receipt.ProductItems.Select(pi => new ProductItemDto
				{
					ID = pi.ID,
					Name = pi.Name,
					Price = pi.Price,
					Quantity = pi.Quantity,
					Sum = pi.Sum,
					NDS = pi.NDS,
					ReceiptID = pi.ReceiptID,
					ProductID = pi.ProductID
				}).ToArray()
			};

			return result;
		}
	}
}
