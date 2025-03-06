using PCSetupHub.Data.Models.Attributes;
using PCSetupHub.Data.Models.Base;
using PCSetupHub.Data.Models.Hardware;

namespace PCSetupHub.Data.Models.Relationships
{
	public class ColorRAM : BaseEntity
	{
		public int ColorID { get; private set; }
		public int RAMID { get; private set; }
		public Color? Color { get; private set; }
		public RAM? RAM { get; private set; }

		public ColorRAM() { }
		public ColorRAM(int colorID, int ramID)
		{
			ColorID = colorID;
			RAMID = ramID;
		}
	}
}