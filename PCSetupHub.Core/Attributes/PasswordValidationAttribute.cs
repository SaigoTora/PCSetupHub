using System.ComponentModel.DataAnnotations;

using PCSetupHub.Core.Utilities;

namespace PCSetupHub.Core.Attributes
{
	public class PasswordValidationAttribute : ValidationAttribute
	{
		private static readonly HashSet<string> _weakPasswords =
		[
			"ab123456", "ab654321", "qw123456", "qw654321", "ab111111", "ab000000", "qw111111",
			"qw000000", "ab123123", "qw123123", "baseball12", "football12", "dragon12",
			"letmein12", "monkey12", "welcome12", "password12", "password123", "admin123",
			"passw0rd1", "passw0rd12", "passw0rd123", "qwerty12", "qwerty123", "qwerty1234",
			"iloveyou12", "1q2w3e4r", "1a2b3c4d", "trustno12", "abc12345", "abcd1234",
			"abcde123", "abcdef12"
		];

		public override bool IsValid(object? value)
		{
			if (value is not string password)
				return false;

			if (password.Length < 8 || password.Length > 64)
				return SetValidationErrorMessage("Password must be between 8 and 50 characters long.");

			if (!Regexes.LowercaseRegex().IsMatch(password))
				return SetValidationErrorMessage("Password must contain at least one lowercase letter.");

			if (!Regexes.UppercaseRegex().IsMatch(password))
				return SetValidationErrorMessage("Password must contain at least one uppercase letter.");

			if (Regexes.DigitRegex().Matches(password).Count < 2)
				return SetValidationErrorMessage("Password must contain at least two digits.");

			if (_weakPasswords.Contains(password.ToLower()))
				return SetValidationErrorMessage("Password is too weak, please choose a stronger one.");

			return true;
		}

		private bool SetValidationErrorMessage(string message)
		{
			ErrorMessage = message;
			return false;
		}
	}
}