using MediatR;
using System;
using WalletKeeper.Application.Dto;

namespace WalletKeeper.Application.Queries
{
	public class GetTokenQuery : IRequest<String>
	{
		public GetTokenQuery(LoginDto dto)
		{
			Dto = dto;
		}

		public LoginDto Dto { get; }
	}
}
