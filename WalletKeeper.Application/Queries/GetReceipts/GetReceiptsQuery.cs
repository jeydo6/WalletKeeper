using MediatR;
using WalletKeeper.Application.Dto;

namespace WalletKeeper.Application.Queries
{
	public class GetReceiptsQuery : IRequest<ReceiptDto[]>
	{
		//
	}
}
