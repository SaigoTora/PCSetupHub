using PCSetupHub.Core.Attributes;
using System.ComponentModel.DataAnnotations;

namespace PCSetupHub.Core.DTOs
{
	public class RegisterRequest
	{
		[Required(ErrorMessage = "Login is required.")]
		[StringLength(32, MinimumLength = 3,
			ErrorMessage = "Login must be between 3 and 20 characters long.")]
		[RegularExpression(@"^[a-zA-Z0-9_.]+$",
			ErrorMessage = "Login can only contain Latin letters, numbers, underscores (_) " +
			"and dots (.).")]
		public required string Login { get; init; }

		[Required(ErrorMessage = "Password is required.")]
		[StringLength(64)]
		[PasswordValidation]
		public required string Password { get; init; }
		[Required(ErrorMessage = "Password confirmation is required.")]
		[Compare("Password", ErrorMessage = "Passwords do not match.")]
		public required string ConfirmPassword { get; init; }

		[Required(ErrorMessage = "Name is required.")]
		[StringLength(64, MinimumLength = 2,
			ErrorMessage = "Username must be between 2 and 64 characters long.")]
		[RegularExpression(@"^[\p{L}]+$",
			ErrorMessage = "Name can only contain letters from any alphabet.")]
		public required string Name { get; init; }

		[Required(ErrorMessage = "Email is required.")]
		[EmailAddress(ErrorMessage = "Invalid email address.")]
		public required string Email { get; init; }

		public RegisterRequest()
		{ }
		public RegisterRequest(string login, string password, string name, string email)
		{
			Login = login;
			Password = password;
			Name = name;
			Email = email;
		}
	}
}