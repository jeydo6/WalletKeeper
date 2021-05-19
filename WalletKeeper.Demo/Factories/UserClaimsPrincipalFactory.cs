using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using WalletKeeper.Demo.DataContexts;
using WalletKeeper.Domain.Entities;
using WalletKeeper.Domain.Exceptions;

namespace WalletKeeper.Demo.Factories
{
	public class UserClaimsPrincipalFactory : IUserClaimsPrincipalFactory<User>
	{
		private readonly ApplicationDataContext _dataContext;

		private readonly ILogger<UserClaimsPrincipalFactory> _logger;

		public UserClaimsPrincipalFactory(
			ApplicationDataContext dataContext,
			ILogger<UserClaimsPrincipalFactory> logger
		)
		{
			_dataContext = dataContext ?? throw new ArgumentNullException(nameof(dataContext));
			_logger = logger ?? throw new ArgumentNullException(nameof(logger));
		}

		public async Task<ClaimsPrincipal> CreateAsync(User user)
		{
			if (user == null)
			{
				throw new BusinessException("User is not exists!");
			}

			var claimsIdentity = GenerateClaimsIdentity(user);
			var claimsPrincipal = new ClaimsPrincipal(claimsIdentity);

			return await Task.FromResult(claimsPrincipal);
		}

		private ClaimsIdentity GenerateClaimsIdentity(User user)
		{
			var identity = new ClaimsIdentity("Identity.Application", ClaimTypes.Name, ClaimTypes.Role);

			identity.AddClaim(new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()));
			identity.AddClaim(new Claim(ClaimTypes.Name, user.UserName));
			identity.AddClaim(new Claim(ClaimTypes.Email, user.Email));

			var roles = _dataContext.UserRoles
				.Where(ur => ur.UserId == user.Id)
				.Join(_dataContext.Roles, ur => ur.RoleId, r => r.Id, (ur, r) => r)
				.Distinct();
			foreach(var role in roles)
			{
				identity.AddClaim(new Claim(ClaimTypes.Role, role.Name));
			}

			return identity;
		}
	}
}
