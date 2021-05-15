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
	public class CreateProductHandler : IRequestHandler<CreateProductCommand, ProductDto>
	{
		private readonly IProductsRepository _repository;
		private readonly ILogger<CreateProductHandler> _logger;

		public CreateProductHandler(
			IProductsRepository repository,
			ILogger<CreateProductHandler> logger
		)
		{
			_repository = repository ?? throw new ArgumentNullException(nameof(repository));
			_logger = logger ?? throw new ArgumentNullException(nameof(logger));
		}

		public async Task<ProductDto> Handle(CreateProductCommand request, CancellationToken cancellationToken)
		{
			if (request.Dto == null)
			{
				throw new ValidationException($"{nameof(request.Dto)} is invalid");
			}

			var item = new Product
			{
				Name = request.Dto.Name,
				CategoryID = request.Dto.CategoryID
			};

			var product = await _repository.CreateAsync(item, cancellationToken);

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
