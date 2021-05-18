using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using System;
using System.Security.Claims;
using System.Threading.Tasks;
using WalletKeeper.Demo.DataContexts;
using WalletKeeper.Domain.Entities;

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
			var claimsPrincipal = new ClaimsPrincipal();

			return await Task.FromResult(claimsPrincipal);
		}
	}
}
