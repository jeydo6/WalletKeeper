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
	public class GetCategoriesHandler : IRequestHandler<GetCategoriesQuery, CategoryDto[]>
	{
		private readonly ApplicationDbContext _dbContext;

		private readonly ILogger<GetCategoriesHandler> _logger;

		public GetCategoriesHandler(
			ApplicationDbContext dbContext,
			ILogger<GetCategoriesHandler> logger
		)
		{
			_dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
			_logger = logger ?? throw new ArgumentNullException(nameof(logger));
		}

		public async Task<CategoryDto[]> Handle(GetCategoriesQuery request, CancellationToken cancellationToken)
		{
			var result = await _dbContext.Categories
				.Select(c => new CategoryDto
				{
					ID = c.ID,
					Name = c.Name
				})
				.ToArrayAsync(cancellationToken);

			return result;
		}
	}
}
