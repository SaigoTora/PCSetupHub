namespace PCSetupHub.Core.Settings
{
	public class TokenSettings
	{
		public required string CookieName { get; set; }
		public TimeSpan Lifetime { get; set; }
	}
}