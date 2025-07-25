using System.ComponentModel.DataAnnotations;

namespace PCSetupHub.Core.DTOs
{
	public class UpdateLoginRequest : PasswordProtectedRequest
	{
		[Required(ErrorMessage = "Login is required.")]
		[StringLength(32, MinimumLength = 3,
			ErrorMessage = "Login must be between 3 and 32 characters long.")]
		[RegularExpression(@"^[a-zA-Z0-9_.]+$",
			ErrorMessage = "Login can only contain Latin letters, numbers, underscores (_) " +
			"and dots (.).")]
		public string NewLogin { get; init; } = string.Empty;

		public UpdateLoginRequest()
		{ }
		public UpdateLoginRequest(string login, bool hasPassword)
			: base(hasPassword)
		{
			NewLogin = login;
		}
	}
}