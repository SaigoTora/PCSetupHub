namespace PCSetupHub.Models.Hardware
{
	public class VideoCard : HardwareComponent
	{
		public string Chipset { get; set; } = string.Empty;
		public float Memory { get; set; }
		public int? CoreClock { get; set; }
		public int? BoostClock { get; set; }
		public string? Color { get; set; }
		public int? Length { get; set; }
		public ICollection<PcConfiguration> PcConfigurations { get; private set; }
			= [];

		public VideoCard() { }
		public VideoCard(string name, bool isDefault, string chipset, int memory, int coreClock,
			int? boostClock, string? color, int length)
			: base(name, isDefault)
		{
			Chipset = chipset;
			Memory = memory;
			CoreClock = coreClock;
			BoostClock = boostClock;
			Color = color;
			Length = length;
		}
	}
}