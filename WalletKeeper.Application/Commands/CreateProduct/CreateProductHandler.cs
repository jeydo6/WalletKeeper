using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;
using WalletKeeper.Application.Dto;
using WalletKeeper.Domain.Entities;
using WalletKeeper.Domain.Exceptions;
using WalletKeeper.Persistence.DbContexts;

namespace WalletKeeper.Application.Commands
{
	public class CreateProductHandler : IRequestHandler<CreateProductCommand, ProductDto>
	{
		private readonly ApplicationDbContext _dbContext;

		private readonly ILogger<CreateProductHandler> _logger;

		public CreateProductHandler(
			ApplicationDbContext dbContext,
			ILogger<CreateProductHandler> logger
		)
		{
			_dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
			_logger = logger ?? throw new ArgumentNullException(nameof(logger));
		}

		public async Task<ProductDto> Handle(CreateProductCommand request, CancellationToken cancellationToken)
		{
			if (request.Dto == null)
			{
				throw new ValidationException($"{nameof(request.Dto)} is invalid");
			}

			var product = await _dbContext.Products.FirstOrDefaultAsync(c => c.Name == request.Dto.Name, cancellationToken);
			if (product != null)
			{
				throw new BusinessException("Product already exists!");
			}

			product = new Product
			{
				Name = request.Dto.Name,
				CategoryID = request.Dto.CategoryID
			};

			await _dbContext.Products.AddAsync(product, cancellationToken);
			await _dbContext.SaveChangesAsync(cancellationToken);

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
