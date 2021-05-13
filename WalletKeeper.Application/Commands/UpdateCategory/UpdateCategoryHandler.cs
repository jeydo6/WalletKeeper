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
	public class UpdateCategoryHandler : IRequestHandler<UpdateCategoryCommand, CategoryDto>
	{
		private readonly ApplicationDbContext _dbContext;
		private readonly ILogger<UpdateCategoryHandler> _logger;

		public UpdateCategoryHandler(
			ApplicationDbContext dbContext,
			ILogger<UpdateCategoryHandler> logger
		)
		{
			_dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
			_logger = logger ?? throw new ArgumentNullException(nameof(logger));
		}

		public async Task<CategoryDto> Handle(UpdateCategoryCommand request, CancellationToken cancellationToken)
		{
			if (request.ID <= 0)
			{
				throw new ValidationException($"{nameof(request.ID)} is invalid");
			}

			if (request.Dto == null)
			{
				throw new ValidationException($"{nameof(request.Dto)} is invalid");
			}

			var category = await _dbContext.Categories.FindAsync(new Object[] { request.ID }, cancellationToken);
			if (category == null)
			{
				throw new BusinessException("Category is not exists!");
			}

			category.Name = request.Dto.Name;

			await _dbContext.SaveChangesAsync(cancellationToken);

			var result = new CategoryDto
			{
				ID = category.ID,
				Name = category.Name
			};

			return result;
		}
	}
}
