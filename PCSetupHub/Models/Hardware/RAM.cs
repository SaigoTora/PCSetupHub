using PCSetupHub.Models.Relationships;

namespace PCSetupHub.Models.Hardware
{
	public class RAM : HardwareComponent
	{
		public byte MemoryType { get; private set; }
		public int Frequency { get; private set; }
		public byte ModulesCount { get; private set; }
		public int ModuleCapacity { get; private set; }
		public string? Color { get; private set; }
		public double FirstWordLatency { get; private set; }
		public int CASLatency { get; private set; }
		public ICollection<PcConfigurationRAM> PcConfigurationRAMs { get; private set; }
			= new HashSet<PcConfigurationRAM>();

		public RAM() { }
		public RAM(string name, bool isDefault, byte memoryType, int frequency, byte modulesCount,
			int moduleCapacity, string? color, double firstWordLatency, int cASLatency)
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