namespace PCSetupHub.Core.Settings
{
	public class AuthSettings
	{
		public TimeSpan Expires { get; set; }
		public required string SecretKey { get; set; }
	}
}