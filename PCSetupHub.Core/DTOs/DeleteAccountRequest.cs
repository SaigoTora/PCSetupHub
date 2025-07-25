namespace PCSetupHub.Core.DTOs
{
	public class DeleteAccountRequest : PasswordProtectedRequest
	{
		public bool IsConfirmed { get; init; } = false;

		public DeleteAccountRequest() : base()
		{ }
		public DeleteAccountRequest(bool hasPassword)
			: base(hasPassword)
		{

		}
	}
}