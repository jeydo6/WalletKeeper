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
	public class UpdateProductHandler : IRequestHandler<UpdateProductCommand, ProductDto>
	{
		private readonly IProductsRepository _repository;
		private readonly ILogger<UpdateProductHandler> _logger;

		public UpdateProductHandler(
			IProductsRepository repository,
			ILogger<UpdateProductHandler> logger
		)
		{
			_repository = repository ?? throw new ArgumentNullException(nameof(repository));
			_logger = logger ?? throw new ArgumentNullException(nameof(logger));
		}

		public async Task<ProductDto> Handle(UpdateProductCommand request, CancellationToken cancellationToken)
		{
			if (request.Dto == null)
			{
				throw new ValidationException($"{nameof(request.Dto)} is invalid");
			}

			if (request.Dto.ID <= 0)
			{
				throw new ValidationException($"{nameof(request.Dto.ID)} is invalid");
			}

			var item = new Product
			{
				Name = request.Dto.Name,
				CategoryID = request.Dto.CategoryID
			};

			var product = await _repository.UpdateAsync(item, cancellationToken);

			var result = new ProductDto
			{
				ID = product.ID,
				Name = product.Name,
				CategoryID = product.CategoryID
			};

			return result;
		}
	}
}
