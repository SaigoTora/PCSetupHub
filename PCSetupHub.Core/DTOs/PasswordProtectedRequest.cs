using System.ComponentModel.DataAnnotations;

namespace PCSetupHub.Core.DTOs
{
	public abstract class PasswordProtectedRequest
	{
		[StringLength(64)]
		public string Password { get; init; } = string.Empty;
		public bool HasPassword { get; private set; } = true;

		protected PasswordProtectedRequest()
		{ }
		protected PasswordProtectedRequest(bool hasPassword)
		{
			HasPassword = hasPassword;
		}

		public bool IsPasswordRequired => HasPassword && string.IsNullOrEmpty(Password);
		public void SetMeta(bool hasPassword) => HasPassword = hasPassword;
	}
}