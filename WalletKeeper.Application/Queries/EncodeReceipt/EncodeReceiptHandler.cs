using MediatR;
using Microsoft.Extensions.Logging;
using System;
using System.ComponentModel.DataAnnotations;
using System.Threading;
using System.Threading.Tasks;
using WalletKeeper.Barcodes.Encoders;

namespace WalletKeeper.Application.Queries
{
	public class EncodeReceiptHandler : IRequestHandler<EncodeReceiptQuery, Byte[]>
	{
		private readonly MagickQRCodeEncoder _barcodeEncoder;

		private readonly ILogger<EncodeReceiptHandler> _logger;

		public EncodeReceiptHandler(
			MagickQRCodeEncoder barcodeEncoder,
			ILogger<EncodeReceiptHandler> logger
		)
		{
			_barcodeEncoder = barcodeEncoder ?? throw new ArgumentNullException(nameof(barcodeEncoder));
			_logger = logger ?? throw new ArgumentNullException(nameof(logger));
		}

		public async Task<Byte[]> Handle(EncodeReceiptQuery request, CancellationToken cancellationToken)
		{
			if (String.IsNullOrWhiteSpace(request.BarcodeString))
			{
				throw new ValidationException($"{nameof(request.BarcodeString)} is invalid");
			}

			var result = _barcodeEncoder.Encode(request.BarcodeString);

			return await Task.FromResult(result);
		}
	}
}
