using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using WalletKeeper.Application.Dto;
using WalletKeeper.Persistence.DbContexts;

namespace WalletKeeper.Application.Queries
{
	public class GetProductsHandler : IRequestHandler<GetProductsQuery, ProductDto[]>
	{
		private readonly ApplicationDbContext _dbContext;

		private readonly ILogger<GetProductsHandler> _logger;

		public GetProductsHandler(
			ApplicationDbContext dbContext,
			ILogger<GetProductsHandler> logger
		)
		{
			_dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
			_logger = logger ?? throw new ArgumentNullException(nameof(logger));
		}

		public async Task<ProductDto[]> Handle(GetProductsQuery request, CancellationToken cancellationToken)
		{
			var result = await _dbContext.Products
				.Select(p => new ProductDto
				{
					ID = p.ID,
					Name = p.Name,
					CategoryID = p.CategoryID
				})
				.ToArrayAsync(cancellationToken);

			return result;
		}
	}
}
