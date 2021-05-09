using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Logging;
using System;
using System.Net;
using WalletKeeper.Application.Dto;
using WalletKeeper.Domain.Exceptions;

namespace WalletKeeper.WebAPI.Filters
{
	/// <summary>
	/// Фильтр ошибок бизнес-логики - <see cref="BusinessException"/>. Возвращает <see cref="ApiErrorDto"/>
	/// </summary>
	public class BusinessExceptionFilter : IExceptionFilter
	{
		private readonly ILogger<BusinessExceptionFilter> _logger;

		public BusinessExceptionFilter(
			ILogger<BusinessExceptionFilter> logger
		)
		{
			_logger = logger;
		}

		public void OnException(ExceptionContext context)
		{
			if (context == null || context.Exception is not BusinessException)
			{
				return;
			}

			var exception = context.Exception as BusinessException;
			var message = exception.Message;
			var code = $"{HttpStatusCode.BadRequest}";

			var apiError = new ApiErrorDto(message, code: code);

			context.Result = new BadRequestObjectResult(apiError);
			context.ExceptionHandled = true;

			_logger.LogError($"Ошибка обработки запроса: {exception}");
		}
	}
}
