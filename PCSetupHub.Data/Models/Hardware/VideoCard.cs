using PCSetupHub.Data.Models.Relationships;

namespace PCSetupHub.Data.Models.Hardware
{
	public class VideoCard : HardwareComponent
	{
		public string Chipset { get; set; } = string.Empty;
		public float Memory { get; set; }
		public int? CoreClock { get; set; }
		public int? BoostClock { get; set; }
		public short? Length { get; set; }
		public ICollection<PcConfiguration>? PcConfigurations { get; private set; }
		public ICollection<ColorVideoCard>? ColorVideoCards { get; private set; }
		public override string DisplayName => $"{Chipset} {Name} {Math.Round(Memory, 2):0.##} GB";

		public VideoCard() { }
		public VideoCard(string name, double? price, bool isDefault, string chipset, float memory,
			int? coreClock, int? boostClock, short? length)
			: base(name, price, isDefault)
		{
			Chipset = chipset;
			Memory = memory;
			CoreClock = coreClock;
			BoostClock = boostClock;
			Length = length;
		}
	}
}