using PCSetupHub.Models.Relationships;

namespace PCSetupHub.Models.Hardware
{
	public class SSD : HardwareComponent
	{
		public int Capacity { get; private set; }
		public string Type { get; private set; } = string.Empty;
		public int? Cache { get; private set; }
		public string FormFactor { get; private set; } = string.Empty;
		public string Interface { get; private set; } = string.Empty;
		public ICollection<PcConfigurationSSD> PcConfigurationSSDs { get; private set; }
			= new HashSet<PcConfigurationSSD>();

		public SSD() { }
		public SSD(string name, bool isDefault, int capacity, string type, int? cache,
			string formFactor, string @interface)
			: base(name, isDefault)
		{
			Capacity = capacity;
			Type = type;
			Cache = cache;
			FormFactor = formFactor;
			Interface = @interface;
		}
	}
}