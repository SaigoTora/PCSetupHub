using PCSetupHub.Data.Models.Base;
using PCSetupHub.Data.Models.Hardware;

namespace PCSetupHub.Data.Models.Relationships
{
	public class PcConfigurationSsd : BaseEntity
	{
		public PcConfiguration? PcConfiguration { get; private set; }
		public int PcConfigurationId { get; private set; }

		public Ssd? Ssd { get; private set; }
		public int SsdId { get; set; }

		public PcConfigurationSsd() { }
		public PcConfigurationSsd(int pcConfigurationId, int ssdId)
		{
			PcConfigurationId = pcConfigurationId;
			SsdId = ssdId;
		}
	}
}