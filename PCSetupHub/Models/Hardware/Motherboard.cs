using PCSetupHub.Models.Relationships;

namespace PCSetupHub.Models.Hardware
{
	public class Motherboard : HardwareComponent
	{
		public string Socket { get; set; } = string.Empty;
		public string FormFactor { get; set; } = string.Empty;
		public int MaxMemory { get; set; }
		public byte MemorySlots { get; set; }
		public ICollection<PcConfiguration>? PcConfigurations { get; private set; }
		public ICollection<ColorMotherboard>? ColorMotherboards { get; private set; }

		public Motherboard() { }
		public Motherboard(string name, double? price, bool isDefault, string socket,
			string formFactor, int maxMemory, byte memorySlots)
			: base(name, price, isDefault)
		{
			Socket = socket;
			FormFactor = formFactor;
			MaxMemory = maxMemory;
			MemorySlots = memorySlots;
		}
	}
}