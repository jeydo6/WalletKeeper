using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Logging;
using System;
using System.ComponentModel.DataAnnotations;
using System.Net;
using WalletKeeper.Application.Dto;

namespace WalletKeeper.WebAPI.Filters
{
	/// <summary>
	/// Фильтр ошибок бизнес-логики - <see cref="ValidationException"/>. Возвращает <see cref="ApiErrorDto"/>
	/// </summary>
	public class ValidationExceptionFilter : IExceptionFilter
	{
		private readonly ILogger<ValidationExceptionFilter> _logger;

		public ValidationExceptionFilter(
			ILogger<ValidationExceptionFilter> logger
		)
		{
			_logger = logger;
		}

		public void OnException(ExceptionContext context)
		{
			if (context == null || context.Exception is not ValidationException)
			{
				return;
			}

			var exception = context.Exception as ValidationException;
			var message = exception.Message;
			var code = $"{(Int32)HttpStatusCode.BadRequest}";

			var apiError = new ApiErrorDto(message, code: code);

			context.Result = new BadRequestObjectResult(apiError);
			context.ExceptionHandled = true;

			_logger.LogError($"Ошибка обработки запроса: {exception}");
		}
	}
}
