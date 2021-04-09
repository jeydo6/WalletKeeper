using System.Threading.Tasks;
using WalletKeeper.Barcodes.Types;
using WalletKeeper.Domain.Entities;

namespace WalletKeeper.Domain.Services
{
	public interface IFiscalDataService
	{
		Task<Receipt> GetReceipt(QRCode qrcode);
	}
}
