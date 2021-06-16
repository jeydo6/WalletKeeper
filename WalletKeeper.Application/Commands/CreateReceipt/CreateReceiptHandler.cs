using MediatR;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Security.Principal;
using System.Threading;
using System.Threading.Tasks;
using WalletKeeper.Application.Dto;
using WalletKeeper.Domain.Entities;
using WalletKeeper.Domain.Exceptions;
using WalletKeeper.Domain.Extensions;
using WalletKeeper.Domain.Repositories;

namespace WalletKeeper.Application.Commands
{
	public class CreateReceiptHandler : IRequestHandler<CreateReceiptCommand, ReceiptDto>
	{
		private readonly IPrincipal _principal;
		private readonly IReceiptsRepository _repository;
		private readonly ILogger<CreateReceiptHandler> _logger;

		public CreateReceiptHandler(
			IPrincipal principal,
			IReceiptsRepository repository,
			ILogger<CreateReceiptHandler> logger
		)
		{
			_principal = principal ?? throw new ArgumentNullException(nameof(principal));
			_repository = repository ?? throw new ArgumentNullException(nameof(repository));
			_logger = logger ?? throw new ArgumentNullException(nameof(logger));
		}

		public async Task<ReceiptDto> Handle(CreateReceiptCommand request, CancellationToken cancellationToken)
		{
			if (request.Dto == null)
			{
				throw new ValidationException($"{nameof(request.Dto)} is invalid");
			}

			if (!Guid.TryParse(_principal.GetUserID(), out var userID))
			{
				throw new BusinessException($"{nameof(userID)} is invalid");
			}

			var existingReceipt = await _repository.FindAsync(request.Dto.FiscalDocumentNumber, request.Dto.FiscalDriveNumber, userID, cancellationToken);
			if (existingReceipt != null)
			{
				throw new BusinessException("Receipt already exists!");
			}

			var item = new Receipt
			{
				FiscalDocumentNumber = request.Dto.FiscalDocumentNumber,
				FiscalDriveNumber = request.Dto.FiscalDriveNumber,
				FiscalType = request.Dto.FiscalType,
				DateTime = request.Dto.DateTime,
				TotalSum = request.Dto.TotalSum,
				OperationType = request.Dto.OperationType,
				Place = request.Dto.Place,
				Organization = new Organization
				{
					INN = request.Dto.Organization.INN,
					Name = request.Dto.Organization.Name
				},
				ProductItems = request.Dto.ProductItems.Select(pi => new ProductItem
				{
					ID = pi.ID,
					Name = pi.Name,
					Price = pi.Price,
					Quantity = pi.Quantity,
					Sum = pi.Sum,
					NDS = pi.NDS,
					UserID = userID
				}).ToList(),
				UserID = userID
			};

			var receipt = await _repository.CreateAsync(item, cancellationToken);

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
