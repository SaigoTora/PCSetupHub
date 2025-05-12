using PCSetupHub.Data.Models.Base;
using PCSetupHub.Data.Models.Hardware;

namespace PCSetupHub.Data.Models.Relationships
{
	public class PcConfigurationHdd : BaseEntity
	{
		public int PcConfigurationId { get; private set; }
		public int HddId { get; private set; }
		public PcConfiguration? PcConfiguration { get; private set; }
		public Hdd? Hdd { get; private set; }

		public PcConfigurationHdd() { }
		public PcConfigurationHdd(int pcConfigurationId, int hddId)
		{
			PcConfigurationId = pcConfigurationId;
			HddId = hddId;
		}
	}
}