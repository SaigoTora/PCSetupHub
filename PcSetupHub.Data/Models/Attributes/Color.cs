using PCSetupHub.Data.Models.Base;
using PCSetupHub.Data.Models.Relationships;

namespace PCSetupHub.Data.Models.Attributes
{
	public class Color : BaseEntity
	{
		public string Name { get; set; } = string.Empty;
		public ICollection<ColorHDD>? ColorHDDs { get; private set; }
		public ICollection<ColorMotherboard>? ColorMotherboards { get; private set; }
		public ICollection<ColorPowerSupply>? ColorPowerSupplies { get; private set; }
		public ICollection<ColorRAM>? ColorRAMs { get; private set; }
		public ICollection<ColorVideoCard>? ColorVideoCards { get; private set; }

		public Color() { }
		public Color(string name)
			=> Name = name;
	}
}