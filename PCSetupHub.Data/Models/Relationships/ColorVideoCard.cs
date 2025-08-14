using PCSetupHub.Data.Models.Attributes;
using PCSetupHub.Data.Models.Base;
using PCSetupHub.Data.Models.Hardware;

namespace PCSetupHub.Data.Models.Relationships
{
	public class ColorVideoCard : BaseEntity
	{
		public Color? Color { get; private set; }
		public int ColorId { get; private set; }

		public VideoCard? VideoCard { get; private set; }
		public int VideoCardId { get; private set; }

		public ColorVideoCard() { }
		public ColorVideoCard(int colorId, int videoCardId)
		{
			ColorId = colorId;
			VideoCardId = videoCardId;
		}
	}
}