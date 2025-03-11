using PCSetupHub.Data.Models.Base;
using PCSetupHub.Data.Models.Hardware;

namespace PCSetupHub.Data.Models.Relationships
{
	public class PcConfigurationRam : BaseEntity
	{
		public int PcConfigurationId { get; private set; }
		public int RamId { get; private set; }
		public PcConfiguration? PcConfiguration { get; private set; }
		public Ram? Ram { get; private set; }

		public PcConfigurationRam() { }
		public PcConfigurationRam(int pcConfigurationId, int ramId)
		{
			PcConfigurationId = pcConfigurationId;
			RamId = ramId;
		}
	}
}