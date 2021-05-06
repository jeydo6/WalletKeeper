using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Logging;
using System;
using System.Net;
using WalletKeeper.Application.Dto;

namespace WalletKeeper.WebAPI.ExceptionFilters
{
	/// <summary>
	/// Фильтр ошибок бизнес-логики - <see cref="ArgumentException"/>. Возвращает <see cref="ApiErrorDto"/>
	/// </summary>
	public class ArgumentExceptionFilter : IExceptionFilter
	{
		private readonly ILogger<ArgumentExceptionFilter> _logger;

		public ArgumentExceptionFilter(
			ILogger<ArgumentExceptionFilter> logger
		)
		{
			_logger = logger;
		}

		public void OnException(ExceptionContext context)
		{
			if (context == null || context.Exception is not ArgumentException)
			{
				return;
			}

			var exception = context.Exception as ArgumentException;
			var message = exception.Message;
			var code = $"{(Int32)HttpStatusCode.BadRequest}";
			var reason = exception.ParamName;

			var apiError = new ApiErrorDto(message, code: code, reason: reason);

			context.Result = new BadRequestObjectResult(apiError);
			context.ExceptionHandled = true;

			_logger.LogError($"Ошибка обработки запроса: {exception}");
		}
	}
}
