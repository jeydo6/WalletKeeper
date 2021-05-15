using MediatR;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Security.Principal;
using System.Threading;
using System.Threading.Tasks;
using WalletKeeper.Application.Dto;
using WalletKeeper.Domain.Exceptions;
using WalletKeeper.Domain.Extensions;
using WalletKeeper.Domain.Repositories;

namespace WalletKeeper.Application.Queries
{
	public class GetReceiptsHandler : IRequestHandler<GetReceiptsQuery, ReceiptDto[]>
	{
		private readonly IPrincipal _principal;
		private readonly IReceiptsRepository _repository;
		private readonly ILogger<GetReceiptsHandler> _logger;

		public GetReceiptsHandler(
			IPrincipal principal,
			IReceiptsRepository repository,
			ILogger<GetReceiptsHandler> logger
		)
		{
			_principal = principal ?? throw new ArgumentNullException(nameof(principal));
			_repository = repository ?? throw new ArgumentNullException(nameof(repository));
			_logger = logger ?? throw new ArgumentNullException(nameof(logger));
		}

		public async Task<ReceiptDto[]> Handle(GetReceiptsQuery request, CancellationToken cancellationToken)
		{
			if (!Guid.TryParse(_principal.GetUserID(), out var userID))
			{
				throw new BusinessException($"{nameof(userID)} is invalid");
			}

			var receipts = await _repository.GetAsync(userID, cancellationToken);

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
