using MediatR;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;
using WalletKeeper.Application.Dto;
using WalletKeeper.Domain.Exceptions;
using WalletKeeper.Persistence.DbContexts;

namespace WalletKeeper.Application.Commands
{
	public class UpdateProductHandler : IRequestHandler<UpdateProductCommand, ProductDto>
	{
		private readonly ApplicationDbContext _dbContext;

		private readonly ILogger<UpdateProductHandler> _logger;

		public UpdateProductHandler(
			ApplicationDbContext dbContext,
			ILogger<UpdateProductHandler> logger
		)
		{
			_dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
			_logger = logger ?? throw new ArgumentNullException(nameof(logger));
		}

		public async Task<ProductDto> Handle(UpdateProductCommand request, CancellationToken cancellationToken)
		{
			if (request.ID <= 0)
			{
				throw new ValidationException($"{nameof(request.ID)} is invalid");
			}

			if (request.Dto == null)
			{
				throw new ValidationException($"{nameof(request.Dto)} is invalid");
			}

			var product = await _dbContext.Products.FindAsync(new Object[] { request.ID }, cancellationToken);
			if (product == null)
			{
				throw new BusinessException("Product is not exists!");
			}

			product.Name = request.Dto.Name;
			product.CategoryID = request.Dto.CategoryID;
			product.Category = null;

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
