using PCSetupHub.Data.Models.Relationships;

namespace PCSetupHub.Data.Models.Hardware
{
	public class RAM : HardwareComponent
	{
		public byte MemoryType { get; set; }
		public int Frequency { get; set; }
		public byte ModulesCount { get; set; }
		public int ModuleCapacity { get; set; }
		public double FirstWordLatency { get; set; }
		public double CASLatency { get; set; }
		public ICollection<PcConfigurationRAM>? PcConfigurationRAMs { get; private set; }
		public ICollection<ColorRAM>? ColorRAMs { get; private set; }

		public RAM() { }
		public RAM(string name, double? price, bool isDefault, byte memoryType, int frequency,
			byte modulesCount, int moduleCapacity, double firstWordLatency, double cASLatency)
			: base(name, price, isDefault)
		{
			MemoryType = memoryType;
			Frequency = frequency;
			ModulesCount = modulesCount;
			ModuleCapacity = moduleCapacity;
			FirstWordLatency = firstWordLatency;
			CASLatency = cASLatency;
		}
	}
}