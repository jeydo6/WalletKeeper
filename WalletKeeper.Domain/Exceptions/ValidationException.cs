using System;
using System.Runtime.Serialization;

namespace WalletKeeper.Domain.Exceptions
{

	/// <summary>
	/// Ошибки валидации
	/// </summary>
	[Serializable]
	public class ValidationException : Exception
	{
		public ValidationException()
		{
			//
		}

		public ValidationException(String message) : base(message)
		{
			//
		}

		public ValidationException(String message, Exception inner) : base(message, inner)
		{
			//
		}

		protected ValidationException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			//
		}
	}
}
