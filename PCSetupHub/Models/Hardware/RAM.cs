using PCSetupHub.Models.Relationships;

namespace PCSetupHub.Models.Hardware
{
	public class RAM : HardwareComponent
	{
		public byte MemoryType { get; set; }
		public int Frequency { get; set; }
		public byte ModulesCount { get; set; }
		public int ModuleCapacity { get; set; }
		public string? Color { get; set; }
		public double FirstWordLatency { get; set; }
		public double CASLatency { get; set; }
		public ICollection<PcConfigurationRAM> PcConfigurationRAMs { get; private set; }
			= [];

		public RAM() { }
		public RAM(string name, bool isDefault, byte memoryType, int frequency, byte modulesCount,
			int moduleCapacity, string? color, double firstWordLatency, double cASLatency)
			: base(name, isDefault)
		{
			MemoryType = memoryType;
			Frequency = frequency;
			ModulesCount = modulesCount;
			ModuleCapacity = moduleCapacity;
			Color = color;
			FirstWordLatency = firstWordLatency;
			CASLatency = cASLatency;
		}
	}
}