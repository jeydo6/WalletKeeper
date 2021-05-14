using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using System;
using System.Security.Principal;
using System.Threading;
using System.Threading.Tasks;
using WalletKeeper.Application.Dto;
using WalletKeeper.Application.Extensions;
using WalletKeeper.Domain.Exceptions;
using WalletKeeper.Persistence.Entities;

namespace WalletKeeper.Application.Queries
{
	public class GetUserHandler : IRequestHandler<GetUserQuery, UserDto>
	{
		private readonly UserManager<User> _userManager;

		private readonly IPrincipal _principal;
		private readonly ILogger<GetUserHandler> _logger;

		public GetUserHandler(
			UserManager<User> userManager,
			IPrincipal principal,
			ILogger<GetUserHandler> logger
		)
		{
			_userManager = userManager ?? throw new ArgumentNullException(nameof(userManager));
			_principal = principal ?? throw new ArgumentNullException(nameof(principal));
			_logger = logger ?? throw new ArgumentNullException(nameof(logger));
		}

		public async Task<UserDto> Handle(GetUserQuery request, CancellationToken cancellationToken)
		{
			var userID = _principal.GetUserID();
			var user = await _userManager.FindByIdAsync(userID);
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
