namespace PCSetupHub.Models.Hardware
{
	public class VideoCard : HardwareComponent
	{
		public string Chipset { get; private set; } = string.Empty;
		public int Memory { get; private set; }
		public int CoreClock { get; private set; }
		public int? BoostClock { get; private set; }
		public string? Color { get; private set; }
		public int Length { get; private set; }
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