using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Security.Principal;
using System.Threading;
using System.Threading.Tasks;
using WalletKeeper.Application.Dto;
using WalletKeeper.Application.Extensions;
using WalletKeeper.Domain.Exceptions;
using WalletKeeper.Persistence.DbContexts;

namespace WalletKeeper.Application.Queries
{
	public class GetReceiptHandler : IRequestHandler<GetReceiptQuery, ReceiptDto>
	{
		private readonly ApplicationDbContext _dbContext;

		private readonly IPrincipal _principal;
		private readonly ILogger<GetReceiptHandler> _logger;

		public GetReceiptHandler(
			ApplicationDbContext dbContext,
			IPrincipal principal,
			ILogger<GetReceiptHandler> logger
		)
		{
			_dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
			_principal = principal ?? throw new ArgumentNullException(nameof(principal));
			_logger = logger ?? throw new ArgumentNullException(nameof(logger));
		}

		public async Task<ReceiptDto> Handle(GetReceiptQuery request, CancellationToken cancellationToken)
		{
			if (request.ID <= 0)
			{
				throw new ValidationException($"{nameof(request.ID)} is invalid");
			}

			if (!Guid.TryParse(_principal.GetUserID(), out var userID))
			{
				throw new BusinessException($"{nameof(userID)} is invalid");
			}

			var receipt = await _dbContext.Receipts.FirstOrDefaultAsync(r => r.ID == request.ID && r.UserID == userID, cancellationToken);
			if (receipt == null)
			{
				throw new BusinessException("ProductItem is not exists!");
			}

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
