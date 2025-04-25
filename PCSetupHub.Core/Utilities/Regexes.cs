using System.Text.RegularExpressions;

namespace PCSetupHub.Core.Utilities
{
	public static partial class Regexes
	{
		[GeneratedRegex("[a-z]")]
		public static partial Regex LowercaseRegex();

		[GeneratedRegex("[A-Z]")]
		public static partial Regex UppercaseRegex();

		[GeneratedRegex("\\d")]
		public static partial Regex DigitRegex();
	}
}