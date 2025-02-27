using PCSetupHub.Models.Attributes;
using PCSetupHub.Models.Base;
using PCSetupHub.Models.Hardware;

namespace PCSetupHub.Models.Relationships
{
	public class ColorMotherboard : BaseEntity
	{
		public int ColorID { get; private set; }
		public int MotherboardID { get; private set; }
		public Color? Color { get; private set; }
		public Motherboard? Motherboard { get; private set; }

		public ColorMotherboard() { }
		public ColorMotherboard(int colorID, int motherboardID)
		{
			ColorID = colorID;
			MotherboardID = motherboardID;
		}
	}
}