using PCSetupHub.Models.Base;
using PCSetupHub.Models.Relationships;

namespace PCSetupHub.Models.Hardware
{
	public class PcConfiguration : BaseEntity
	{
		public Processor? Processor { get; private set; }
		public VideoCard? VideoCard { get; private set; }
		public Motherboard? Motherboard { get; private set; }
		public PowerSupply? PowerSupply { get; private set; }
		public int ProcessorID { get; private set; }
		public int VideoCardID { get; private set; }
		public int MotherboardID { get; private set; }
		public int PowerSupplyID { get; private set; }
		public ICollection<PcConfigurationSSD> PcConfigurationSSDs { get; private set; }
			= new HashSet<PcConfigurationSSD>();
		public ICollection<PcConfigurationHDD> PcConfigurationHDDs { get; private set; }
			= new HashSet<PcConfigurationHDD>();
		public ICollection<PcConfigurationRAM> PcConfigurationRAMs { get; private set; }
			= new HashSet<PcConfigurationRAM>();

		public PcConfiguration() { }
		public PcConfiguration(Processor? processor, VideoCard? videoCard,
			Motherboard? motherboard, PowerSupply? powerSupply)
		{
			Processor = processor;
			VideoCard = videoCard;
			Motherboard = motherboard;
			PowerSupply = powerSupply;
		}
	}
}