using MediatR;
using System;
using WalletKeeper.Application.Dto;

namespace WalletKeeper.Application.Queries
{
	public class GetUserTokenQuery : IRequest<String>
	{
		public GetUserTokenQuery(LoginDto dto)
		{
			Dto = dto;
		}

		public LoginDto Dto { get; }
	}
}
