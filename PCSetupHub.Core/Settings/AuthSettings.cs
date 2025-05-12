namespace PCSetupHub.Core.Settings
{
	public class AuthSettings
	{
		public required TokenSettings AccessToken { get; set; }
		public required TokenSettings RefreshToken { get; set; }
	}
}