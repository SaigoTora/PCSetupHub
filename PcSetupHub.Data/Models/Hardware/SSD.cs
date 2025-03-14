using PCSetupHub.Data.Models.Relationships;

namespace PCSetupHub.Data.Models.Hardware
{
	public class Ssd : HardwareComponent
	{
		public double Capacity { get; set; }
		public string Type { get; set; } = string.Empty;
		public int? Cache { get; set; }
		public string FormFactor { get; set; } = string.Empty;
		public string Interface { get; set; } = string.Empty;
		public ICollection<PcConfigurationSsd>? PcConfigurationSsds { get; private set; }

		public Ssd() { }
		public Ssd(string name, double? price, bool isDefault, float capacity, string type,
			int? cache, string formFactor, string @interface)
			: base(name, price, isDefault)
		{
			Capacity = capacity;
			Type = type;
			Cache = cache;
			FormFactor = formFactor;
			Interface = @interface;
		}
	}
}