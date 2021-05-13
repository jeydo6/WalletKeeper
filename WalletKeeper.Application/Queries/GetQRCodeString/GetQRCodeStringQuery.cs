using MediatR;
using System;

namespace WalletKeeper.Application.Queries
{
	public class GetQRCodeStringQuery : IRequest<String>
	{
		public GetQRCodeStringQuery(Byte[] photo)
		{
			Photo = photo;
		}

		public Byte[] Photo { get; }
	}
}
