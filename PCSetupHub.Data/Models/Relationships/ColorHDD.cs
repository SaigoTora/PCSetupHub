using PCSetupHub.Data.Models.Attributes;
using PCSetupHub.Data.Models.Base;
using PCSetupHub.Data.Models.Hardware;

namespace PCSetupHub.Data.Models.Relationships
{
	public class ColorHdd : BaseEntity
	{
		public int ColorId { get; private set; }
		public int HddId { get; private set; }
		public Color? Color { get; private set; }
		public Hdd? Hdd { get; private set; }

		public ColorHdd() { }
		public ColorHdd(int colorId, int hddId)
		{
			ColorId = colorId;
			HddId = hddId;
		}
	}
}