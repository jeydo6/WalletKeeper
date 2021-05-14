using MediatR;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using WalletKeeper.Application.Dto;
using WalletKeeper.Domain.Repositories;

namespace WalletKeeper.Application.Queries
{
	public class GetReceiptsHandler : IRequestHandler<GetReceiptsQuery, ReceiptDto[]>
	{
		private readonly IReceiptsRepository _repository;
		private readonly ILogger<GetReceiptsHandler> _logger;

		public GetReceiptsHandler(
			IReceiptsRepository repository,
			ILogger<GetReceiptsHandler> logger
		)
		{
			_repository = repository ?? throw new ArgumentNullException(nameof(repository));
			_logger = logger ?? throw new ArgumentNullException(nameof(logger));
		}

		public async Task<ReceiptDto[]> Handle(GetReceiptsQuery request, CancellationToken cancellationToken)
		{
			var receipts = await _repository.GetAsync(cancellationToken);

			var result = receipts
				.Select(r => new ReceiptDto
				{
					FiscalDocumentNumber = r.FiscalDocumentNumber,
					FiscalDriveNumber = r.FiscalDriveNumber,
					FiscalType = r.FiscalType,
					DateTime = r.DateTime,
					TotalSum = r.TotalSum,
					OperationType = r.OperationType
				})
				.ToArray();

			return result;
		}
	}
}
