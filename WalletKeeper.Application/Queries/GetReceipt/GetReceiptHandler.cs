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
	public class GetReceiptHandler : IRequestHandler<GetReceiptQuery, ReceiptDto>
	{
		private readonly IPrincipal _principal;
		private readonly IReceiptsRepository _repository;
		private readonly ILogger<GetReceiptHandler> _logger;

		public GetReceiptHandler(
			IPrincipal principal,
			IReceiptsRepository repository,
			ILogger<GetReceiptHandler> logger
		)
		{
			_principal = principal ?? throw new ArgumentNullException(nameof(principal));
			_repository = repository ?? throw new ArgumentNullException(nameof(repository));
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

			var receipt = await _repository.GetAsync(request.ID, userID, cancellationToken);
			if (receipt == null)
			{
				throw new BusinessException("ProductItem is not exists!");
			}

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
