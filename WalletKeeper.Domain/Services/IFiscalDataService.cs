using System.Collections.Generic;
using System.Threading.Tasks;
using WalletKeeper.Domain.Entities;

namespace WalletKeeper.Domain.Services
{
	public interface IFiscalDataService
	{
		Task<Receipt> GetReceipt(ReceiptHeader receiptHeader);
	}
}
