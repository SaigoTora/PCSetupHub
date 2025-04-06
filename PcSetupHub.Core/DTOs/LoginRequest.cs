using System.ComponentModel.DataAnnotations;

namespace PCSetupHub.Core.DTOs
{
	public class LoginRequest
	{
		[Required(ErrorMessage = "Login is required.")]
		[StringLength(32)]
		public required string Login { get; init; }
		[Required(ErrorMessage = "Password is required.")]
		[StringLength(64)]
		public required string Password { get; init; }
		public required bool RememberMe { get; init; } = true;
	}
}