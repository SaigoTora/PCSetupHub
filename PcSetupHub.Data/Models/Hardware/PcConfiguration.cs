using PCSetupHub.Data.Models.Base;
using PCSetupHub.Data.Models.Relationships;
using PCSetupHub.Data.Models.Users;

namespace PCSetupHub.Data.Models.Hardware
{
	public class PcConfiguration : BaseEntity
	{
		public PcType? Type { get; private set; }
		public Processor? Processor { get; private set; }
		public VideoCard? VideoCard { get; private set; }
		public Motherboard? Motherboard { get; private set; }
		public PowerSupply? PowerSupply { get; private set; }
		public int? TypeId { get; private set; }
		public int? ProcessorId { get; private set; }
		public int? VideoCardId { get; private set; }
		public int? MotherboardId { get; private set; }
		public int? PowerSupplyId { get; private set; }
		public User? User { get; private set; }
		public ICollection<PcConfigurationSsd>? PcConfigurationSsds { get; private set; }
		public ICollection<PcConfigurationHdd>? PcConfigurationHdds { get; private set; }
		public ICollection<PcConfigurationRam>? PcConfigurationRams { get; private set; }
		public double? TotalPrice
		{
			get
			{
				double price = (Processor?.Price ?? 0) + (VideoCard?.Price ?? 0) + (Motherboard?.Price ?? 0)
					+ (PowerSupply?.Price ?? 0);

				if (PcConfigurationRams != null)
					price += PcConfigurationRams.Sum(ram => ram.Ram?.Price ?? 0);
				if (PcConfigurationSsds != null)
					price += PcConfigurationSsds.Sum(ssd => ssd.Ssd?.Price ?? 0);
				if (PcConfigurationHdds != null)
					price += PcConfigurationHdds.Sum(hdd => hdd.Hdd?.Price ?? 0);

				return price == 0 ? null : price;
			}
		}
		public bool AreAllComponentsDefault
		{
			get
			{
				static bool IsDefaultOrNull(HardwareComponent? component) =>
					component == null || component.IsDefault;

				if (!IsDefaultOrNull(Processor)) return false;
				if (!IsDefaultOrNull(VideoCard)) return false;
				if (!IsDefaultOrNull(Motherboard)) return false;
				if (!IsDefaultOrNull(PowerSupply)) return false;

				if (PcConfigurationRams?.Any(x => !IsDefaultOrNull(x?.Ram)) == true) return false;
				if (PcConfigurationSsds?.Any(x => !IsDefaultOrNull(x?.Ssd)) == true) return false;
				if (PcConfigurationHdds?.Any(x => !IsDefaultOrNull(x?.Hdd)) == true) return false;

				return true;
			}
		}

		public PcConfiguration() { }
		public PcConfiguration(int? typeId, int? processorId, int? videoCardId,
			int? motherboardId, int? powerSupplyId)
		{
			TypeId = typeId;
			ProcessorId = processorId;
			VideoCardId = videoCardId;
			MotherboardId = motherboardId;
			PowerSupplyId = powerSupplyId;
		}
	}
}