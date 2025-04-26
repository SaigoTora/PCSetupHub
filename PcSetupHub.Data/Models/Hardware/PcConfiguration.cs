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