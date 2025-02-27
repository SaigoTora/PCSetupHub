using PCSetupHub.Models.Attributes;
using PCSetupHub.Models.Base;
using PCSetupHub.Models.Hardware;

namespace PCSetupHub.Models.Relationships
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