using System.Globalization;

namespace WalletKeeper.Domain.Constants
{
	public static class GlobalizationConstants
	{
		public static readonly NumberFormatInfo DOT_POINT_SEPARATOR = new NumberFormatInfo
		{
			NumberDecimalSeparator = "."
		};

		public static readonly NumberFormatInfo COMMA_POINT_SEPARATOR = new NumberFormatInfo
		{
			NumberDecimalSeparator = ","
		};
	}
}
