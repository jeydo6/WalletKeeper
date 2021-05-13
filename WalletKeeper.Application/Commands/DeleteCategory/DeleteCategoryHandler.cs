using MediatR;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;
using WalletKeeper.Domain.Exceptions;
using WalletKeeper.Persistence.DbContexts;

namespace WalletKeeper.Application.Commands
{
	public class DeleteCategoryHandler : IRequestHandler<DeleteCategoryCommand>
	{
		private readonly ApplicationDbContext _dbContext;

		private readonly ILogger<DeleteCategoryHandler> _logger;

		public DeleteCategoryHandler(
			ApplicationDbContext dbContext,
			ILogger<DeleteCategoryHandler> logger
		)
		{
			_dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
			_logger = logger ?? throw new ArgumentNullException(nameof(logger));
		}

		public async Task<Unit> Handle(DeleteCategoryCommand request, CancellationToken cancellationToken)
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

			_dbContext.Categories.Remove(category);
			await _dbContext.SaveChangesAsync(cancellationToken);

			return Unit.Value;
		}
	}
}
