using PCSetupHub.Models.Attributes;
using PCSetupHub.Models.Base;
using PCSetupHub.Models.Hardware;

namespace PCSetupHub.Models.Relationships
{
	public class ColorPowerSupply : BaseEntity
	{
		public int ColorID { get; private set; }
		public int PowerSupplyID { get; private set; }
		public Color? Color { get; private set; }
		public PowerSupply? PowerSupply { get; private set; }

		public ColorPowerSupply() { }
		public ColorPowerSupply(int colorID, int powerSupplyID)
		{
			ColorID = colorID;
			PowerSupplyID = powerSupplyID;
		}
	}
}