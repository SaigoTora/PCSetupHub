using PCSetupHub.Models.Relationships;

namespace PCSetupHub.Models.Hardware
{
	public class PowerSupply : HardwareComponent
	{
		public string Type { get; set; } = string.Empty;
		public string? Efficiency { get; set; }
		public int Wattage { get; set; }
		public string Modular { get; set; } = string.Empty;
		public ICollection<PcConfiguration>? PcConfigurations { get; private set; }
		public ICollection<ColorPowerSupply>? ColorPowerSupplies { get; private set; }

		public PowerSupply() { }
		public PowerSupply(string name, double? price, bool isDefault,
			string type, string? efficiency, int wattage, string modular)
			: base(name, price, isDefault)
		{
			Type = type;
			Efficiency = efficiency;
			Wattage = wattage;
			Modular = modular;
		}
	}
}