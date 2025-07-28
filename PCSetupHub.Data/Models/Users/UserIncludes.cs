namespace PCSetupHub.Data.Models.Users
{
	[Flags]
	public enum UserIncludes
	{
		None = 0,
		PrivacySetting = 1 << 0,
		PcConfiguration = 1 << 1,
		PcConfigurationFull = 1 << 2,
		Friendships = 1 << 3,
		Messages = 1 << 4
	}
}