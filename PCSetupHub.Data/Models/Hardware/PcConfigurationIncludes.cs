namespace PCSetupHub.Data.Models.Hardware
{
	[Flags]
	public enum PcConfigurationIncludes
	{
		None = 0,
		UserLogin = 1 << 0,
		Processor = 1 << 1,
		VideoCard = 1 << 2,
		Motherboard = 1 << 3,
		PowerSupply = 1 << 4,
		Ssds = 1 << 5,
		Rams = 1 << 6,
		Hdds = 1 << 7
	}
}