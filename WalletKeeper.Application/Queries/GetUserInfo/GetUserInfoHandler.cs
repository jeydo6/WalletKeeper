using MediatR;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using WalletKeeper.Application.Dto;

namespace WalletKeeper.Application.Queries
{
	public class GetUserInfoHandler : IRequestHandler<GetUserInfoQuery, UserInfoDto>
	{
		public async Task<UserInfoDto> Handle(GetUserInfoQuery request, CancellationToken cancellationToken)
		{
			var result = new UserInfoDto
			{
				IsAuthenticated = true,
				Claims = request.User.Claims
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
