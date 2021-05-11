using MediatR;
using System.Security.Claims;
using WalletKeeper.Application.Dto;

namespace WalletKeeper.Application.Queries
{
	public class GetUserInfoQuery : IRequest<UserInfoDto>
	{
		public GetUserInfoQuery(ClaimsPrincipal user)
		{
			User = user;
		}

		public ClaimsPrincipal User { get; }
	}
}
