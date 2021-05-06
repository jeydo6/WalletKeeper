using System;

namespace WalletKeeper.Application.Dto
{
	public class ApiErrorDto
	{
		public ApiErrorDto()
		{
			//
		}

		public ApiErrorDto(String message, String code = null, String reason = null)
		{
			Message = message;
			Code = code;
			Reason = reason;
		}

		/// <summary>
		/// Сообщение об ошибке
		/// </summary>
		public String Message { get; set; }

		/// <summary>
		/// Код ошибки
		/// </summary>
		public String Code { get; set; }

		/// <summary>
		/// Причина ошибки
		/// </summary>
		public String Reason { get; set; }
	}
}
