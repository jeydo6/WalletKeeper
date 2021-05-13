using MediatR;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;
using WalletKeeper.Domain.Exceptions;
using WalletKeeper.Persistence.DbContexts;

namespace WalletKeeper.Application.Commands
{
	public class DeleteProductHandler : IRequestHandler<DeleteProductCommand>
	{
		private readonly ApplicationDbContext _dbContext;

		private readonly ILogger<DeleteProductHandler> _logger;

		public DeleteProductHandler(
			ApplicationDbContext dbContext,
			ILogger<DeleteProductHandler> logger
		)
		{
			_dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
			_logger = logger ?? throw new ArgumentNullException(nameof(logger));
		}

		public async Task<Unit> Handle(DeleteProductCommand request, CancellationToken cancellationToken)
		{
			if (request.ID <= 0)
			{
				throw new ValidationException($"{nameof(request.ID)} is invalid");
			}

			var product = await _dbContext.Products.FindAsync(request.ID);
			if (product == null)
			{
				throw new BusinessException("Product is not exists!");
			}

			_dbContext.Products.Remove(product);
			await _dbContext.SaveChangesAsync(cancellationToken);

			return Unit.Value;
		}
	}
}
