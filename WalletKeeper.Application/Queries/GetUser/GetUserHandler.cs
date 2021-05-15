using MediatR;
using Microsoft.Extensions.Logging;
using System;
using System.Security.Principal;
using System.Threading;
using System.Threading.Tasks;
using WalletKeeper.Application.Dto;
using WalletKeeper.Domain.Exceptions;
using WalletKeeper.Domain.Extensions;
using WalletKeeper.Domain.Repositories;

namespace WalletKeeper.Application.Queries
{
	public class GetUserHandler : IRequestHandler<GetUserQuery, UserDto>
	{
		private readonly IPrincipal _principal;
		private readonly IUsersRepository _usersRepository;
		private readonly ILogger<GetUserHandler> _logger;

		public GetUserHandler(
			IPrincipal principal,
			IUsersRepository usersRepository,
			ILogger<GetUserHandler> logger
		)
		{
			_principal = principal ?? throw new ArgumentNullException(nameof(principal));
			_usersRepository = usersRepository ?? throw new ArgumentNullException(nameof(usersRepository));
			_logger = logger ?? throw new ArgumentNullException(nameof(logger));
		}

		public async Task<UserDto> Handle(GetUserQuery request, CancellationToken cancellationToken)
		{
			var userID = _principal.GetUserID();
			var user = await _usersRepository.GetAsync(userID);
			if (user == null)
			{
				throw new BusinessException("User is not exists!");
			}

			var result = new UserDto
			{
				ID = user.Id,
				UserName = user.UserName,
				Email = user.Email,
				EmailConfirmed = user.EmailConfirmed
			};

			return result;
		}
	}
}
