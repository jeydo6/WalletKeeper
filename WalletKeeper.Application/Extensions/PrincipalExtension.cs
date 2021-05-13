using System;
using System.Security.Claims;
using System.Security.Principal;
using WalletKeeper.Domain.Exceptions;

namespace WalletKeeper.Application.Extensions
{
	public static class PrincipalExtension
	{
		public static String GetUserID(this IPrincipal principal)
		{
			if (principal is not ClaimsPrincipal claimsPrincipal)
			{
				throw new BusinessException($"{nameof(claimsPrincipal)} is invalid");
			}

			var userID = claimsPrincipal.FindFirstValue(ClaimTypes.NameIdentifier);

			if (String.IsNullOrWhiteSpace(userID))
			{
				throw new BusinessException($"{nameof(userID)} is invalid");
			}

			return userID;
		}
	}
}
