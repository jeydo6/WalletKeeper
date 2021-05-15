using MediatR;
using Microsoft.Extensions.Logging;
using System;
using System.Security.Principal;
using System.Threading;
using System.Threading.Tasks;
using WalletKeeper.Application.Dto;
using WalletKeeper.Domain.Exceptions;
using WalletKeeper.Domain.Extensions;
using WalletKeeper.Domain.Repositories;
using WalletKeeper.Domain.Services;
using WalletKeeper.Domain.Types;

namespace WalletKeeper.Application.Commands
{
	public class CreateReceiptHandler : IRequestHandler<CreateReceiptCommand, ReceiptDto>
	{
		private readonly IPrincipal _principal;
		private readonly IReceiptsRepository _repository;
		private readonly IFiscalDataService _fiscalDataService;
		private readonly ILogger<CreateReceiptHandler> _logger;

		public CreateReceiptHandler(
			IPrincipal principal,
			IReceiptsRepository repository,
			IFiscalDataService fiscalDataService,
			ILogger<CreateReceiptHandler> logger
		)
		{
			_principal = principal ?? throw new ArgumentNullException(nameof(principal));
			_repository = repository ?? throw new ArgumentNullException(nameof(repository));
			_fiscalDataService = fiscalDataService ?? throw new ArgumentNullException(nameof(fiscalDataService));
			_logger = logger ?? throw new ArgumentNullException(nameof(logger));
		}

		public async Task<ReceiptDto> Handle(CreateReceiptCommand request, CancellationToken cancellationToken)
		{
			if (String.IsNullOrWhiteSpace(request.BarcodeString))
			{
				throw new ValidationException($"{nameof(request.BarcodeString)} is invalid");
			}

			var qrcode = QRCode.Parse(request.BarcodeString);

			var receipt = await _repository.FindAsync(qrcode.FiscalDocumentNumber, cancellationToken);
			if (receipt != null)
			{
				throw new BusinessException("Receipt already exists!");
			}

			if (!Guid.TryParse(_principal.GetUserID(), out var userID))
			{
				throw new BusinessException($"{nameof(userID)} is invalid");
			}

			var item = await _fiscalDataService.GetReceipt(qrcode);

			receipt = await _repository.CreateAsync(item, userID, cancellationToken);

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
