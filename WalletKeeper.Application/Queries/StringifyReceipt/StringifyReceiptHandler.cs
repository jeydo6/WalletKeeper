using MediatR;
using System;
using System.Threading;
using System.Threading.Tasks;
using WalletKeeper.Domain.Constants;
using WalletKeeper.Domain.Types;

namespace WalletKeeper.Application.Queries
{
	public class StringifyReceiptHandler : IRequestHandler<StringifyReceiptQuery, String>
	{
		public async Task<String> Handle(StringifyReceiptQuery request, CancellationToken cancellationToken)
		{
			var qrcode = new QRCode
			{
				FiscalDocumentNumber = request.Dto.FiscalDocumentNumber,
				FiscalDriveNumber = request.Dto.FiscalDriveNumber,
				FiscalType = request.Dto.FiscalType,
				DateTime = request.Dto.DateTime.ToString("yyyyMMddTHHmm"),
				TotalSum = request.Dto.TotalSum.ToString(GlobalizationConstants.DOT_POINT_SEPARATOR),
				OperationType = request.Dto.OperationType.ToString()
			};

			var result = qrcode.ToString();

			return await Task.FromResult(result);
		}
	}
}
