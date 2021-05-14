using MediatR;
using System;

namespace WalletKeeper.Application.Commands
{
	public class DeleteReceiptCommand : IRequest
	{
		public DeleteReceiptCommand(Int32 id)
		{
			ID = id;
		}

		public Int32 ID { get; }
	}
}
