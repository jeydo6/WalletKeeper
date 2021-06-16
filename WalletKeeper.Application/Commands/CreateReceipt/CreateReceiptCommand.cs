using MediatR;
using WalletKeeper.Application.Dto;

namespace WalletKeeper.Application.Commands
{
	public class CreateReceiptCommand : IRequest<ReceiptDto>
	{
		public CreateReceiptCommand(ReceiptDto dto)
		{
			Dto = dto;
		}

		public ReceiptDto Dto { get; }
	}
}
