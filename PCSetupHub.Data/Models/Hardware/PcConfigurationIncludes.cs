namespace PCSetupHub.Data.Models.Hardware
{
	[Flags]
	public enum PcConfigurationIncludes
	{
		None = 0,
		Processor = 1 << 0,
		VideoCard = 1 << 1,
		Motherboard = 1 << 2,
		PowerSupply = 1 << 3,
		Ssds = 1 << 4,
		Rams = 1 << 5,
		Hdds = 1 << 6
	}
}