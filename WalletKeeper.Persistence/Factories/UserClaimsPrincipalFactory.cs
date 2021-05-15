using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using System.Security.Claims;
using System.Threading.Tasks;
using WalletKeeper.Domain.Entities;

namespace WalletKeeper.Persistence.Factories
{
	public class UserClaimsPrincipalFactory : UserClaimsPrincipalFactory<User>
	{
		public UserClaimsPrincipalFactory(
			UserManager<User> userManager,
			IOptions<IdentityOptions> optionsAccessor
		) : base(userManager, optionsAccessor)
		{
			//
		}

		protected override async Task<ClaimsIdentity> GenerateClaimsAsync(User user)
		{
			var identity = await base.GenerateClaimsAsync(user);

			if (identity != null)
			{
				foreach (var role in await UserManager.GetRolesAsync(user))
				{
					identity.AddClaim(new Claim(ClaimTypes.Role, role));
				}

				return identity;
			}

			return null;
		}
	}
}