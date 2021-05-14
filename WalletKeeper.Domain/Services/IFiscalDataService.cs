using System.Threading.Tasks;
using WalletKeeper.Domain.Entities;
using WalletKeeper.Domain.Types;

namespace WalletKeeper.Domain.Services
{
	public interface IFiscalDataService
	{
		Task<Receipt> GetReceipt(QRCode qrcode);
	}
}
