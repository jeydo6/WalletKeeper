using MediatR;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;
using WalletKeeper.Application.Dto;
using WalletKeeper.Domain.Entities;
using WalletKeeper.Domain.Exceptions;
using WalletKeeper.Domain.Repositories;

namespace WalletKeeper.Application.Commands
{
	public class UpdateProductItemHandler : IRequestHandler<UpdateProductItemCommand, ProductItemDto>
	{
		private readonly IProductItemsRepository _repository;
		private readonly ILogger<UpdateProductItemHandler> _logger;

		public UpdateProductItemHandler(
			IProductItemsRepository repository,
			ILogger<UpdateProductItemHandler> logger
		)
		{
			_repository = repository ?? throw new ArgumentNullException(nameof(repository));
			_logger = logger ?? throw new ArgumentNullException(nameof(logger));
		}

		public async Task<ProductItemDto> Handle(UpdateProductItemCommand request, CancellationToken cancellationToken)
		{
			if (request.ID <= 0)
			{
				throw new ValidationException($"{nameof(request.ID)} is invalid");
			}

			if (request.Dto == null)
			{
				throw new ValidationException($"{nameof(request.Dto)} is invalid");
			}

			var item = new ProductItem
			{
				ID = request.Dto.ID,
				Name = request.Dto.Name,
				Price = request.Dto.Price,
				Quantity = request.Dto.Quantity,
				Sum = request.Dto.Sum,
				ProductID = request.Dto.ProductID,
				ReceiptID = request.Dto.ReceiptID
			};

			var productItem = await _repository.UpdateAsync(request.ID, item, cancellationToken);

			var result = new ProductItemDto
			{
				ID = productItem.ID,
				Name = productItem.Name,
				Price = productItem.Price,
				Quantity = productItem.Quantity,
				Sum = productItem.Sum,
				ProductID = productItem.ProductID,
				ReceiptID = productItem.ReceiptID
			};

			return result;
		}
	}
}
