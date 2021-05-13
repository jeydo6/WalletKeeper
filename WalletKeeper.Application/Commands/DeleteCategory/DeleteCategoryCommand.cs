using MediatR;
using System;

namespace WalletKeeper.Application.Commands
{
	public class DeleteCategoryCommand : IRequest
	{
		public DeleteCategoryCommand(Int32 id)
		{
			ID = id;
		}

		public Int32 ID { get; }
	}
}
