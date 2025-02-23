using PCSetupHub.Models.Base;
using PCSetupHub.Models.Hardware;

namespace PCSetupHub.Models.Relationships
{
	public class PcConfigurationHDD : BaseEntity
	{
		public int PcConfigurationID { get; private set; }
		public int HDDID { get; private set; }
		public PcConfiguration? PcConfiguration { get; private set; }
		public HDD? HDD { get; private set; }

		public PcConfigurationHDD() { }
		public PcConfigurationHDD(int pcConfigurationID, int hddID)
		{
			PcConfigurationID = pcConfigurationID;
			HDDID = hddID;
		}
	}
}