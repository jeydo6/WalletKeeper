using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Logging;
using System;
using System.Net;
using WalletKeeper.Application.Dto;

namespace WalletKeeper.WebAPI.Filters
{
	/// <summary>
	/// Фильтр ошибок бизнес-логики - <see cref="Exception"/>. Возвращает <see cref="ApiErrorDto"/>
	/// </summary>
	public class ExceptionFilter : IExceptionFilter
	{
		private readonly ILogger<ExceptionFilter> _logger;

		public ExceptionFilter(
			ILogger<ExceptionFilter> logger
		)
		{
			_logger = logger;
		}

		public void OnException(ExceptionContext context)
		{
			if (context == null)
			{
				return;
			}

			var exception = context.Exception;
			var message = exception.Message;
			var code = $"{HttpStatusCode.InternalServerError}";

			var apiError = new ApiErrorDto(message, code: code);

			context.Result = new ObjectResult(apiError)
			{
				StatusCode = (Int32)HttpStatusCode.InternalServerError
			};
			context.ExceptionHandled = true;

			_logger.LogError(exception, "An error occurred");
		}
	}
}
