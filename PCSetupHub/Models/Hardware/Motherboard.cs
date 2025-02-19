namespace PCSetupHub.Models.Hardware
{
	public class Motherboard : HardwareComponent
	{
		public string Socket { get; private set; } = string.Empty;
		public string FormFactor { get; private set; } = string.Empty;
		public int MaxMemory { get; private set; }
		public byte MemorySlots { get; private set; }
		public string? Color { get; private set; }

		public Motherboard() { }
		public Motherboard(string name, bool isDefault, string socket, string formFactor,
			int maxMemory, byte memorySlots, string? color)
			: base(name, isDefault)
		{
			Socket = socket;
			FormFactor = formFactor;
			MaxMemory = maxMemory;
			MemorySlots = memorySlots;
			Color = color;
		}
	}
}