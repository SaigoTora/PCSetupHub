using PCSetupHub.Models.Relationships;

namespace PCSetupHub.Models.Hardware
{
	public class HDD : HardwareComponent
	{
		public string Type { get; private set; } = string.Empty;
		public string? Interface { get; private set; }
		public int Capacity { get; private set; }
		public string? Color { get; private set; }
		public ICollection<PcConfigurationHDD> PcConfigurationHDDs { get; private set; }
			= new HashSet<PcConfigurationHDD>();

		public HDD() { }
		public HDD(string name, bool isDefault, string type, string? @interface,
			int capacity, string? color)
			: base(name, isDefault)
		{
			Type = type;
			Interface = @interface;
			Capacity = capacity;
			Color = color;
		}
	}
}