using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using WalletKeeper.Domain.Configs;
using WalletKeeper.Domain.Entities;
using WalletKeeper.Domain.Exceptions;
using WalletKeeper.Domain.Repositories;

namespace WalletKeeper.Application.Queries
{
	public class GetUserTokenHandler : IRequestHandler<GetUserTokenQuery, String>
	{
		private readonly AuthenticationConfig _authenticationConfig;

		private readonly IUsersRepository _usersRepository;
		private readonly IUserClaimsPrincipalFactory<User> _claimsPrincipalFactory;
		private readonly ILogger<GetUserTokenHandler> _logger;

		public GetUserTokenHandler(
			UserManager<User> userManager,
			IOptionsSnapshot<AuthenticationConfig> authenticationConfigOptions,
			IUsersRepository usersRepository,
			IUserClaimsPrincipalFactory<User> claimsPrincipalFactory,
			ILogger<GetUserTokenHandler> logger
		)
		{
			_authenticationConfig = authenticationConfigOptions != null ? authenticationConfigOptions.Value : throw new ArgumentNullException(nameof(authenticationConfigOptions));
			_usersRepository = usersRepository ?? throw new ArgumentNullException(nameof(usersRepository));
			_claimsPrincipalFactory = claimsPrincipalFactory ?? throw new ArgumentNullException(nameof(claimsPrincipalFactory));
			_logger = logger ?? throw new ArgumentNullException(nameof(logger));
		}

		public async Task<String> Handle(GetUserTokenQuery request, CancellationToken cancellationToken)
		{
			if (String.IsNullOrWhiteSpace(request.Dto.UserName))
			{
				throw new ValidationException("UserName cannot be empty!");
			}

			if (String.IsNullOrEmpty(request.Dto.Password))
			{
				throw new ValidationException("Password cannot be empty!");
			}

			var user = await _usersRepository.FindAsync(request.Dto.UserName);
			if(user == null || !await _usersRepository.CheckPasswordAsync(user, request.Dto.Password))
			{
				throw new BusinessException("Invalid username or password");
			}

			var identity = await _claimsPrincipalFactory.CreateAsync(user);
			if (identity == null)
			{
				throw new BusinessException("Invalid username or password");
			}

			var key = new SymmetricSecurityKey(
				Encoding.UTF8.GetBytes(_authenticationConfig.Secret)
			);

			var jwt = new JwtSecurityToken(
				issuer: _authenticationConfig.Issuer,
				notBefore: DateTime.UtcNow,
				expires: DateTime.UtcNow.AddDays(14),
				claims: identity.Claims,
				signingCredentials: new SigningCredentials(key, SecurityAlgorithms.HmacSha256)
			);

			var result = new JwtSecurityTokenHandler()
					.WriteToken(jwt);

			return result;
		}
	}
}
