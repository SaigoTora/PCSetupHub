using System.ComponentModel.DataAnnotations;

using PCSetupHub.Data.Models.Base;
using PCSetupHub.Data.Models.Relationships;

namespace PCSetupHub.Data.Models.Attributes
{
	public class Color : BaseEntity
	{
		[StringLength(64)]
		public string Name { get; set; } = string.Empty;

		public ICollection<ColorHdd>? ColorHdds { get; private set; }
		public ICollection<ColorMotherboard>? ColorMotherboards { get; private set; }
		public ICollection<ColorPowerSupply>? ColorPowerSupplies { get; private set; }
		public ICollection<ColorRam>? ColorRams { get; private set; }
		public ICollection<ColorVideoCard>? ColorVideoCards { get; private set; }

		public Color() { }
		public Color(string name)
			=> Name = name;
	}
}