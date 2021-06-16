using MediatR;
using Microsoft.Extensions.Logging;
using System;
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
	public class CreateProductHandler : IRequestHandler<CreateProductCommand, ProductDto>
	{
		private readonly IPrincipal _principal;
		private readonly IProductsRepository _repository;
		private readonly ILogger<CreateProductHandler> _logger;

		public CreateProductHandler(
			IPrincipal principal,
			IProductsRepository repository,
			ILogger<CreateProductHandler> logger
		)
		{
			_principal = principal ?? throw new ArgumentNullException(nameof(principal));
			_repository = repository ?? throw new ArgumentNullException(nameof(repository));
			_logger = logger ?? throw new ArgumentNullException(nameof(logger));
		}

		public async Task<ProductDto> Handle(CreateProductCommand request, CancellationToken cancellationToken)
		{
			if (request.Dto == null)
			{
				throw new ValidationException($"{nameof(request.Dto)} is invalid");
			}

			if (!Guid.TryParse(_principal.GetUserID(), out var userID))
			{
				throw new BusinessException($"{nameof(userID)} is invalid");
			}

			var item = new Product
			{
				Name = request.Dto.Name,
				CategoryID = request.Dto.CategoryID,
				UserID = userID
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
