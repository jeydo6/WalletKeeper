using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Security.Principal;
using System.Threading;
using System.Threading.Tasks;
using WalletKeeper.Application.Dto;
using WalletKeeper.Application.Extensions;
using WalletKeeper.Domain.Exceptions;
using WalletKeeper.Persistence.DbContexts;

namespace WalletKeeper.Application.Queries
{
	public class GetReceiptsHandler : IRequestHandler<GetReceiptsQuery, ReceiptDto[]>
	{
		private readonly ApplicationDbContext _dbContext;

		private readonly IPrincipal _principal;
		private readonly ILogger<GetReceiptsHandler> _logger;

		public GetReceiptsHandler(
			ApplicationDbContext dbContext,
			IPrincipal principal,
			ILogger<GetReceiptsHandler> logger
		)
		{
			_dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
			_principal = principal ?? throw new ArgumentNullException(nameof(principal));
			_logger = logger ?? throw new ArgumentNullException(nameof(logger));
		}

		public async Task<ReceiptDto[]> Handle(GetReceiptsQuery request, CancellationToken cancellationToken)
		{
			if (!Guid.TryParse(_principal.GetUserID(), out var userID))
			{
				throw new BusinessException($"{nameof(userID)} is invalid");
			}

			var result = await _dbContext.Receipts
				.Where(r => r.UserID == userID)
				.Select(r => new ReceiptDto
				{
					FiscalDocumentNumber = r.FiscalDocumentNumber,
					FiscalDriveNumber = r.FiscalDriveNumber,
					FiscalType = r.FiscalType,
					DateTime = r.DateTime,
					TotalSum = r.TotalSum,
					OperationType = r.OperationType
				})
				.ToArrayAsync(cancellationToken);

			return result;
		}
	}
}
