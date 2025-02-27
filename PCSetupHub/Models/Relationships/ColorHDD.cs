using PCSetupHub.Models.Attributes;
using PCSetupHub.Models.Base;
using PCSetupHub.Models.Hardware;

namespace PCSetupHub.Models.Relationships
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