using Microsoft.Extensions.Configuration;
using System;
using System.IO;
using System.Threading.Tasks;
using WalletKeeper.Barcodes.Decoders;
using WalletKeeper.Barcodes.Enumerations;
using WalletKeeper.Domain.Entities;

namespace WalletKeeper.ConsoleApp
{
	class Program
	{
		static async Task Main(String[] args)
		{
			var filePath = $"{Directory.GetCurrentDirectory()}\\qrcode.jpg";

			var decoder = new MagickBarcodeDecoder(BarcodeFormatEnum.QR_CODE);

			var image = await File.ReadAllBytesAsync(filePath);
			var result = decoder.Decode(image);

			var receiptHeader = ReceiptHeader.Parse(result);
		}
	}
}
