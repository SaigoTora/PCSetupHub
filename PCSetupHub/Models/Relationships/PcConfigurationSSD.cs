using PCSetupHub.Models.Base;
using PCSetupHub.Models.Hardware;

namespace PCSetupHub.Models.Relationships
{
	public class PcConfigurationSSD : BaseEntity
	{
		public int PcConfigurationID { get; private set; }
		public int SSDID { get; private set; }
		public PcConfiguration? PcConfiguration { get; private set; }
		public SSD? SSD { get; private set; }

		public PcConfigurationSSD() { }
		public PcConfigurationSSD(int pcConfigurationID, int ssdID)
		{
			PcConfigurationID = pcConfigurationID;
			SSDID = ssdID;
		}
	}
}