namespace PCSetupHub.Models.Hardware
{
	public class PowerSupply : HardwareComponent
	{
		public string Type { get; set; } = string.Empty;
		public string? Efficiency { get; set; }
		public int Wattage { get; set; }
		public string Modular { get; set; } = string.Empty;
		public string? Color { get; set; }
		public ICollection<PcConfiguration>? PcConfigurations { get; private set; }

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