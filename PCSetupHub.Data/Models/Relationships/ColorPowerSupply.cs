using PCSetupHub.Data.Models.Attributes;
using PCSetupHub.Data.Models.Base;
using PCSetupHub.Data.Models.Hardware;

namespace PCSetupHub.Data.Models.Relationships
{
	public class ColorPowerSupply : BaseEntity
	{
		public int ColorId { get; private set; }
		public int PowerSupplyId { get; private set; }
		public Color? Color { get; private set; }
		public PowerSupply? PowerSupply { get; private set; }

		public ColorPowerSupply() { }
		public ColorPowerSupply(int colorId, int powerSupplyId)
		{
			ColorId = colorId;
			PowerSupplyId = powerSupplyId;
		}
	}
}