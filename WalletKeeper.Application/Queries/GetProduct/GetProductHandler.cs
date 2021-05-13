using MediatR;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;
using WalletKeeper.Application.Dto;
using WalletKeeper.Domain.Exceptions;
using WalletKeeper.Persistence.DbContexts;

namespace WalletKeeper.Application.Queries
{
	public class GetProductHandler : IRequestHandler<GetProductQuery, ProductDto>
	{
		private readonly ApplicationDbContext _dbContext;

		private readonly ILogger<GetProductHandler> _logger;

		public GetProductHandler(
			ApplicationDbContext dbContext,
			ILogger<GetProductHandler> logger
		)
		{
			_dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
			_logger = logger ?? throw new ArgumentNullException(nameof(logger));
		}

		public async Task<ProductDto> Handle(GetProductQuery request, CancellationToken cancellationToken)
		{
			if (request.ID <= 0)
			{
				throw new ValidationException($"{nameof(request.ID)} is invalid");
			}

			var product = await _dbContext.Products.FindAsync(new Object[] { request.ID }, cancellationToken);
			if (product == null)
			{
				throw new BusinessException("Product is not exists!");
			}

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
