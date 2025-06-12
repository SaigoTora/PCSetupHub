using PCSetupHub.Data.Models.Relationships;

namespace PCSetupHub.Data.Models.Hardware
{
	public class Hdd : HardwareComponent
	{
		public string Type { get; set; } = string.Empty;
		public string? Interface { get; set; }
		public int Capacity { get; set; }
		public ICollection<PcConfigurationHdd>? PcConfigurationHdds { get; private set; }
		public ICollection<ColorHdd>? ColorHdds { get; private set; }
		public override string DisplayName => $"{Name} {Capacity} GB";

		public Hdd() { }
		public Hdd(string name, double? price, bool isDefault,
			string type, string? @interface, int capacity)
			: base(name, price, isDefault)
		{
			Type = type;
			Interface = @interface;
			Capacity = capacity;
		}
	}
}