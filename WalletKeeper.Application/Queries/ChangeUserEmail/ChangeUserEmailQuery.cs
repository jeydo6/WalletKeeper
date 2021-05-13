using MediatR;
using System;

namespace WalletKeeper.Application.Queries
{
	public class ChangeUserEmailQuery : IRequest
	{
		public ChangeUserEmailQuery(String email)
		{
			Email = email;
		}

		public String Email { get; }
	}
}
