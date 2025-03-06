using PCSetupHub.Data.Models.Attributes;
using PCSetupHub.Data.Models.Base;
using PCSetupHub.Data.Models.Hardware;

namespace PCSetupHub.Data.Models.Relationships
{
	public class ColorVideoCard : BaseEntity
	{
		public int ColorID { get; private set; }
		public int VideoCardID { get; private set; }
		public Color? Color { get; private set; }
		public VideoCard? VideoCard { get; private set; }

		public ColorVideoCard() { }
		public ColorVideoCard(int colorID, int videoCardID)
		{
			ColorID = colorID;
			VideoCardID = videoCardID;
		}
	}
}