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
	public class GetCategoryHandler : IRequestHandler<GetCategoryQuery, CategoryDto>
	{
		private readonly ApplicationDbContext _dbContext;

		private readonly ILogger<GetCategoryHandler> _logger;

		public GetCategoryHandler(
			ApplicationDbContext dbContext,
			ILogger<GetCategoryHandler> logger
		)
		{
			_dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
			_logger = logger ?? throw new ArgumentNullException(nameof(logger));
		}

		public async Task<CategoryDto> Handle(GetCategoryQuery request, CancellationToken cancellationToken)
		{
			if (request.ID <= 0)
			{
				throw new ValidationException($"{nameof(request.ID)} is invalid");
			}

			var category = await _dbContext.Categories.FindAsync(new Object[] { request.ID }, cancellationToken);
			if (category == null)
			{
				throw new BusinessException("Category is not exists!");
			}

			var result = new CategoryDto
			{
				ID = category.ID,
				Name = category.Name
			};

			return result;
		}
	}
}
