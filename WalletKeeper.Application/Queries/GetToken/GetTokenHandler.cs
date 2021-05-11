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

namespace WalletKeeper.Application.Queries
{
	public class GetTokenHandler : IRequestHandler<GetTokenQuery, String>
	{
		private readonly UserManager<User> _userManager;
		private readonly AuthenticationConfig _authenticationConfig;
		private readonly IUserClaimsPrincipalFactory<User> _claimsPrincipalFactory;
		private readonly ILogger<GetTokenHandler> _logger;

		public GetTokenHandler(
			UserManager<User> userManager,
			IOptionsSnapshot<AuthenticationConfig> authenticationConfigOptions,
			IUserClaimsPrincipalFactory<User> claimsPrincipalFactory,
			ILogger<GetTokenHandler> logger
		)
		{
			_userManager = userManager ?? throw new ArgumentNullException(nameof(userManager));
			_authenticationConfig = authenticationConfigOptions != null ? authenticationConfigOptions.Value : throw new ArgumentNullException(nameof(authenticationConfigOptions));
			_claimsPrincipalFactory = claimsPrincipalFactory ?? throw new ArgumentNullException(nameof(claimsPrincipalFactory));
			_logger = logger ?? throw new ArgumentNullException(nameof(logger));
		}

		public async Task<String> Handle(GetTokenQuery request, CancellationToken cancellationToken)
		{
			if (String.IsNullOrWhiteSpace(request.Dto.UserName))
			{
				throw new ValidationException("UserName cannot be empty!");
			}

			if (String.IsNullOrEmpty(request.Dto.Password))
			{
				throw new ValidationException("Password cannot be empty!");
			}

			var user = await _userManager.FindByNameAsync(request.Dto.UserName);
			if (user == null || !await _userManager.CheckPasswordAsync(user, request.Dto.Password))
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
