using System.ComponentModel.DataAnnotations;

using PCSetupHub.Data.Models.Relationships;

namespace PCSetupHub.Data.Models.Hardware
{
	public class Ram : HardwareComponent
	{
		[Range(0, byte.MaxValue, ErrorMessage = "Memory type must be greater or equal than 0.")]
		public byte MemoryType { get; set; }

		[Range(333, int.MaxValue, ErrorMessage = "Frequency must be at least 333 MHz.")]
		public int Frequency { get; set; }

		[Range(1, byte.MaxValue, ErrorMessage = "Modules count must be greater than 0.")]
		public byte ModulesCount { get; set; }

		[Range(1, int.MaxValue, ErrorMessage = "Module capacity must be at least 1 GB.")]
		public int ModuleCapacity { get; set; }

		[Range(6, double.MaxValue, ErrorMessage = "First Word Latency must be at least 6 ns.")]
		public double FirstWordLatency { get; set; }

		[Range(1, double.MaxValue, ErrorMessage = "CAS Latency must be greater than 0.")]
		public double CASLatency { get; set; }

		public ICollection<PcConfigurationRam>? PcConfigurationRams { get; private set; }
		public ICollection<ColorRam>? ColorRams { get; set; }

		public override string DisplayName => $"{Name} {DisplayMemoryType}";
		private string DisplayMemoryType => "DDR" + (MemoryType == 0 ? " (DDR1)" : MemoryType);

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