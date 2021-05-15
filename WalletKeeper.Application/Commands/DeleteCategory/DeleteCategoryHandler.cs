using MediatR;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;
using WalletKeeper.Domain.Exceptions;
using WalletKeeper.Domain.Repositories;

namespace WalletKeeper.Application.Commands
{
	public class DeleteCategoryHandler : IRequestHandler<DeleteCategoryCommand>
	{
		private readonly ICategoriesRepository _repository;
		private readonly ILogger<DeleteCategoryHandler> _logger;

		public DeleteCategoryHandler(
			ICategoriesRepository repository,
			ILogger<DeleteCategoryHandler> logger
		)
		{
			_repository = repository ?? throw new ArgumentNullException(nameof(repository));
			_logger = logger ?? throw new ArgumentNullException(nameof(logger));
		}

		public async Task<Unit> Handle(DeleteCategoryCommand request, CancellationToken cancellationToken)
		{
			if (request.ID <= 0)
			{
				throw new ValidationException($"{nameof(request.ID)} is invalid");
			}

			await _repository.DeleteAsync(request.ID, cancellationToken);

			return Unit.Value;
		}
	}
}
