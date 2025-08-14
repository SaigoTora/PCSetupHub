using PCSetupHub.Data.Models.Attributes;
using PCSetupHub.Data.Models.Base;
using PCSetupHub.Data.Models.Hardware;

namespace PCSetupHub.Data.Models.Relationships
{
	public class ColorMotherboard : BaseEntity
	{
		public Color? Color { get; private set; }
		public int ColorId { get; private set; }

		public Motherboard? Motherboard { get; private set; }
		public int MotherboardId { get; private set; }

		public ColorMotherboard() { }
		public ColorMotherboard(int colorId, int motherboardId)
		{
			ColorId = colorId;
			MotherboardId = motherboardId;
		}
	}
}