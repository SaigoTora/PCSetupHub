namespace PCSetupHub.Data.Models.Users
{
	[Flags]
	public enum UserIncludes
	{
		None = 0,
		Password = 1 << 0,
		PrivacySetting = 1 << 1,
		PcConfiguration = 1 << 2,
		PcConfigurationFull = 1 << 3,
		Friendships = 1 << 4,
		Messages = 1 << 5
	}
}