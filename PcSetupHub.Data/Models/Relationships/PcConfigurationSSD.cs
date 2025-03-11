using PCSetupHub.Data.Models.Base;
using PCSetupHub.Data.Models.Hardware;

namespace PCSetupHub.Data.Models.Relationships
{
	public class PcConfigurationSsd : BaseEntity
	{
		public int PcConfigurationId { get; private set; }
		public int SsdId { get; private set; }
		public PcConfiguration? PcConfiguration { get; private set; }
		public Ssd? Ssd { get; private set; }

		public PcConfigurationSsd() { }
		public PcConfigurationSsd(int pcConfigurationId, int ssdId)
		{
			PcConfigurationId = pcConfigurationId;
			SsdId = ssdId;
		}
	}
}