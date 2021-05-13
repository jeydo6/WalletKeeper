using MediatR;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Security.Claims;
using System.Security.Principal;
using System.Threading;
using System.Threading.Tasks;
using WalletKeeper.Application.Dto;
using WalletKeeper.Domain.Exceptions;

namespace WalletKeeper.Application.Queries
{
	public class GetUserInfoHandler : IRequestHandler<GetUserInfoQuery, UserInfoDto>
	{
		private readonly IPrincipal _principal;
		private readonly ILogger<GetUserInfoHandler> _logger;

		public GetUserInfoHandler(
			IPrincipal principal,
			ILogger<GetUserInfoHandler> logger
		)
		{
			_principal = principal ?? throw new ArgumentNullException(nameof(principal));
			_logger = logger ?? throw new ArgumentNullException(nameof(logger));
		}

		public async Task<UserInfoDto> Handle(GetUserInfoQuery request, CancellationToken cancellationToken)
		{
			if (_principal is not ClaimsPrincipal claimsPrincipal)
			{
				throw new BusinessException($"{nameof(claimsPrincipal)} is invalid");
			}

			var result = new UserInfoDto
			{
				IsAuthenticated = true,
				Claims = claimsPrincipal.Claims
					.Select(c => new ClaimDto
					{
						Type = c.Type,
						Value = c.Value,
						ValueType = c.ValueType,
						Issuer = c.Issuer,
						OriginalIssuer = c.OriginalIssuer
					})
					.ToArray()
			};

			return await Task.FromResult(result);
		}
	}
}
