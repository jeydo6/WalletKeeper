using MediatR;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;
using WalletKeeper.Barcodes.Decoders;

namespace WalletKeeper.Application.Queries
{
	public class DecodeReceiptHandler : IRequestHandler<DecodeReceiptQuery, String>
	{
		private readonly MagickQRCodeDecoder _barcodeDecoder;

		private readonly ILogger<DecodeReceiptHandler> _logger;

		public DecodeReceiptHandler(
			MagickQRCodeDecoder barcodeDecoder,
			ILogger<DecodeReceiptHandler> logger
		)
		{
			_barcodeDecoder = barcodeDecoder ?? throw new ArgumentNullException(nameof(barcodeDecoder));
			_logger = logger ?? throw new ArgumentNullException(nameof(logger));
		}

		public async Task<String> Handle(DecodeReceiptQuery request, CancellationToken cancellationToken)
		{
			var result = _barcodeDecoder.Decode(request.Photo);

			return await Task.FromResult(result);
		}
	}
}
