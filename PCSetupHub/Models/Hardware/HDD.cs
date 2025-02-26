using PCSetupHub.Models.Relationships;

namespace PCSetupHub.Models.Hardware
{
	public class HDD : HardwareComponent
	{
		public string Type { get; set; } = string.Empty;
		public string? Interface { get; set; }
		public int Capacity { get; set; }
		public string? Color { get; set; }
		public ICollection<PcConfigurationHDD>? PcConfigurationHDDs { get; private set; }

		public HDD() { }
		public HDD(string name, double? price, bool isDefault, string type, string? @interface,
			int capacity, string? color)
			: base(name, price, isDefault)
		{
			Type = type;
			Interface = @interface;
			Capacity = capacity;
			Color = color;
		}
	}
}