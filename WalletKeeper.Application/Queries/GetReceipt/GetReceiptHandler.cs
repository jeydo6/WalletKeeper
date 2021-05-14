using MediatR;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;
using WalletKeeper.Application.Dto;
using WalletKeeper.Domain.Exceptions;
using WalletKeeper.Domain.Repositories;

namespace WalletKeeper.Application.Queries
{
	public class GetReceiptHandler : IRequestHandler<GetReceiptQuery, ReceiptDto>
	{
		private readonly IReceiptsRepository _repository;
		private readonly ILogger<GetReceiptHandler> _logger;

		public GetReceiptHandler(
			IReceiptsRepository repository,
			ILogger<GetReceiptHandler> logger
		)
		{
			_repository = repository ?? throw new ArgumentNullException(nameof(repository));
			_logger = logger ?? throw new ArgumentNullException(nameof(logger));
		}

		public async Task<ReceiptDto> Handle(GetReceiptQuery request, CancellationToken cancellationToken)
		{
			if (request.ID <= 0)
			{
				throw new ValidationException($"{nameof(request.ID)} is invalid");
			}

			var receipt = await _repository.GetAsync(request.ID, cancellationToken);

			var result = new ReceiptDto
			{
				FiscalDocumentNumber = receipt.FiscalDocumentNumber,
				FiscalDriveNumber = receipt.FiscalDriveNumber,
				FiscalType = receipt.FiscalType,
				DateTime = receipt.DateTime,
				TotalSum = receipt.TotalSum,
				OperationType = receipt.OperationType
			};

			return result;

		}
	}
}
