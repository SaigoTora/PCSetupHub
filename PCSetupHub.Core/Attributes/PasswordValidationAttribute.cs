using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

namespace PCSetupHub.Core.Attributes
{
	public class PasswordValidationAttribute : ValidationAttribute
	{
		public override bool IsValid(object? value)
		{
			if (value is not string password)
				return false;

			if (password.Length < 8 || password.Length > 64)
			{
				ErrorMessage = "Password must be between 8 and 50 characters long.";
				return false;
			}

			if (!Regex.IsMatch(password, @"[a-z]"))
			{
				ErrorMessage = "Password must contain at least one lowercase letter.";
				return false;
			}

			if (!Regex.IsMatch(password, @"[A-Z]"))
			{
				ErrorMessage = "Password must contain at least one uppercase letter.";
				return false;
			}

			if (Regex.Matches(password, @"\d").Count < 2)
			{
				ErrorMessage = "Password must contain at least two digits.";
				return false;
			}

			return true;
		}
	}
}