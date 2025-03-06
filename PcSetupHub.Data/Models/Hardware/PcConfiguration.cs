using PCSetupHub.Data.Models.Base;
using PCSetupHub.Data.Models.Relationships;
using PCSetupHub.Data.Models.Users;

namespace PCSetupHub.Data.Models.Hardware
{
	public class PcConfiguration : BaseEntity
	{
		public Processor? Processor { get; private set; }
		public VideoCard? VideoCard { get; private set; }
		public Motherboard? Motherboard { get; private set; }
		public PowerSupply? PowerSupply { get; private set; }
		public int? ProcessorID { get; private set; }
		public int? VideoCardID { get; private set; }
		public int? MotherboardID { get; private set; }
		public int? PowerSupplyID { get; private set; }
		public User? User { get; private set; }
		public ICollection<PcConfigurationSSD>? PcConfigurationSSDs { get; private set; }
		public ICollection<PcConfigurationHDD>? PcConfigurationHDDs { get; private set; }
		public ICollection<PcConfigurationRAM>? PcConfigurationRAMs { get; private set; }

		public PcConfiguration() { }
		public PcConfiguration(int? processorID, int? videoCardID,
			int? motherboardID, int? powerSupplyID)
		{
			ProcessorID = processorID;
			VideoCardID = videoCardID;
			MotherboardID = motherboardID;
			PowerSupplyID = powerSupplyID;
		}
	}
}