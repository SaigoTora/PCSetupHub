using PCSetupHub.Data.Models.Attributes;
using PCSetupHub.Data.Models.Base;
using PCSetupHub.Data.Models.Hardware;

namespace PCSetupHub.Data.Models.Relationships
{
	public class ColorHDD : BaseEntity
	{
		public int ColorID { get; private set; }
		public int HDDID { get; private set; }
		public Color? Color { get; private set; }
		public HDD? HDD { get; private set; }

		public ColorHDD() { }
		public ColorHDD(int colorID, int hddID)
		{
			ColorID = colorID;
			HDDID = hddID;
		}
	}
}