using MediatR;
using System;

namespace WalletKeeper.Application.Queries
{
	public class DecodeQRCodeQuery : IRequest<String>
	{
		public DecodeQRCodeQuery(Byte[] photo)
		{
			Photo = photo;
		}

		public Byte[] Photo { get; }
	}
}
