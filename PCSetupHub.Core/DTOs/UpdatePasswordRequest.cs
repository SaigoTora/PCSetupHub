using System.ComponentModel.DataAnnotations;

using PCSetupHub.Core.Attributes;

namespace PCSetupHub.Core.DTOs
{
	public class UpdatePasswordRequest
	{
		[Required(ErrorMessage = "Old Password is required.")]
		[StringLength(64)]
		public string OldPassword { get; init; } = string.Empty;

		[Required(ErrorMessage = "New Password is required.")]
		[StringLength(64)]
		[PasswordValidation]
		public string NewPassword { get; init; } = string.Empty;

		[Required(ErrorMessage = "Password confirmation is required.")]
		[StringLength(64)]
		[Compare(nameof(NewPassword), ErrorMessage = "New password and confirmation do not match.")]
		public string ConfirmPassword { get; init; } = string.Empty;
	}
}