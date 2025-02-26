using PCSetupHub.Models.Base;

namespace PCSetupHub.Models.Hardware
{
	public class HardwareComponent : BaseEntity
	{
		public string Name { get; set; } = string.Empty;
		public double? Price { get; set; }
		public bool IsDefault { get; set; }

		public HardwareComponent() { }
		public HardwareComponent(string name, double? price, bool isDefault)
		{
			Name = name;
			Price = price;
			IsDefault = isDefault;
		}
	}
}