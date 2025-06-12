using PCSetupHub.Data.Models.Relationships;

namespace PCSetupHub.Data.Models.Hardware
{
	public class Ram : HardwareComponent
	{
		public byte MemoryType { get; set; }
		public int Frequency { get; set; }
		public byte ModulesCount { get; set; }
		public int ModuleCapacity { get; set; }
		public double FirstWordLatency { get; set; }
		public double CASLatency { get; set; }
		public ICollection<PcConfigurationRam>? PcConfigurationRams { get; private set; }
		public ICollection<ColorRam>? ColorRams { get; private set; }
		public override string DisplayName => $"{Name} DDR{MemoryType}";

		public Ram() { }
		public Ram(string name, double? price, bool isDefault, byte memoryType, int frequency,
			byte modulesCount, int moduleCapacity, double firstWordLatency, double casLatency)
			: base(name, price, isDefault)
		{
			MemoryType = memoryType;
			Frequency = frequency;
			ModulesCount = modulesCount;
			ModuleCapacity = moduleCapacity;
			FirstWordLatency = firstWordLatency;
			CASLatency = casLatency;
		}
	}
}