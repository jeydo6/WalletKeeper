using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using WalletKeeper.Domain.Exceptions;

namespace WalletKeeper.Persistence.Extensions
{
	public static class IdentityResultExtension
	{
		public static void EnsureSuccess(this IdentityResult identityResult, String errorMessage, ILogger logger)
		{
			if (!identityResult.Succeeded)
			{
				var fullError = String.Join(
					", ",
					identityResult.Errors
						.Select(e => $"[{e.Code}] {e.Description}")
						.ToArray()
				);

				logger.LogDebug($"{errorMessage}: {fullError}");

				var shortError = String.Join(
					", ",
					identityResult.Errors
						.Select(e => e.Description)
						.ToArray()
				);

				throw new BusinessException(shortError);
			}
		}
	}
}
