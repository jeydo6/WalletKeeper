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
	public class CreateCategoryHandler : IRequestHandler<CreateCategoryCommand, CategoryDto>
	{
		private readonly ApplicationDbContext _dbContext;

		private readonly ILogger<CreateCategoryHandler> _logger;

		public CreateCategoryHandler(
			ApplicationDbContext dbContext,
			ILogger<CreateCategoryHandler> logger
		)
		{
			_dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
			_logger = logger ?? throw new ArgumentNullException(nameof(logger));
		}

		public async Task<CategoryDto> Handle(CreateCategoryCommand request, CancellationToken cancellationToken)
		{
			if (request.Dto == null)
			{
				throw new ValidationException($"{nameof(request.Dto)} is invalid");
			}

			var category = await _dbContext.Categories.FirstOrDefaultAsync(c => c.Name == request.Dto.Name, cancellationToken);
			if (category != null)
			{
				throw new BusinessException("Category already exists!");
			}

			category = new Category
			{
				Name = request.Dto.Name
			};

			await _dbContext.Categories.AddAsync(category, cancellationToken);
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
