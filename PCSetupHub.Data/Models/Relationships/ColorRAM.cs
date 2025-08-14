using PCSetupHub.Data.Models.Attributes;
using PCSetupHub.Data.Models.Base;
using PCSetupHub.Data.Models.Hardware;

namespace PCSetupHub.Data.Models.Relationships
{
	public class ColorRam : BaseEntity
	{
		public Color? Color { get; private set; }
		public int ColorId { get; private set; }

		public Ram? Ram { get; private set; }
		public int RamId { get; private set; }

		public ColorRam() { }
		public ColorRam(int colorId, int ramId)
		{
			ColorId = colorId;
			RamId = ramId;
		}
	}
}