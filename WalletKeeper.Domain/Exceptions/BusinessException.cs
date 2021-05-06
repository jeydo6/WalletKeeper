using System;
using System.Runtime.Serialization;

namespace WalletKeeper.Domain.Exceptions
{
	/// <summary>
	/// Ошибки бизнес-логики
	/// </summary>
	[Serializable]
	public class BusinessException : Exception
	{
		public BusinessException()
		{
			//
		}

		public BusinessException(String message) : base(message)
		{
			//
		}

		public BusinessException(String message, Exception inner) : base(message, inner)
		{
			//
		}

		protected BusinessException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			//
		}
	}
}
