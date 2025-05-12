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