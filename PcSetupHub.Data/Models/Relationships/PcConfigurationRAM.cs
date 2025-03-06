using PCSetupHub.Data.Models.Base;
using PCSetupHub.Data.Models.Hardware;

namespace PCSetupHub.Data.Models.Relationships
{
	public class PcConfigurationRAM : BaseEntity
	{
		public int PcConfigurationID { get; private set; }
		public int RAMID { get; private set; }
		public PcConfiguration? PcConfiguration { get; private set; }
		public RAM? RAM { get; private set; }

		public PcConfigurationRAM() { }
		public PcConfigurationRAM(int pcConfigurationID, int ramID)
		{
			PcConfigurationID = pcConfigurationID;
			RAMID = ramID;
		}
	}
}