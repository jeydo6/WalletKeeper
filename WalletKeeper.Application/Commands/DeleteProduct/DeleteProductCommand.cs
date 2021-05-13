using MediatR;
using System;

namespace WalletKeeper.Application.Commands
{
	public class DeleteProductCommand : IRequest
	{
		public DeleteProductCommand(Int32 id)
		{
			ID = id;
		}

		public Int32 ID { get; }
	}
}
