using MediatR;
using System;

namespace WalletKeeper.Application.Queries
{
	public class DecodeReceiptQuery : IRequest<String>
	{
		public DecodeReceiptQuery(Byte[] photo)
		{
			Photo = photo;
		}

		public Byte[] Photo { get; }
	}
}
