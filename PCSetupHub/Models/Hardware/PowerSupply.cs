namespace PCSetupHub.Models.Hardware
{
	public class PowerSupply : HardwareComponent
	{
		public string Type { get; private set; } = string.Empty;
		public string? Efficiency { get; private set; }
		public int Wattage { get; private set; }
		public string Modular { get; private set; } = string.Empty;
		public string? Color { get; private set; }
		public ICollection<PcConfiguration> PcConfigurations { get; private set; }
			= [];

		public PowerSupply() { }
		public PowerSupply(string name, bool isDefault, string type, string? efficiency,
			int wattage, string modular, string? color)
			: base(name, isDefault)
		{
			Type = type;
			Efficiency = efficiency;
			Wattage = wattage;
			Modular = modular;
			Color = color;
		}
	}
}