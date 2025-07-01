using System.ComponentModel.DataAnnotations;

using PCSetupHub.Data.Models.Relationships;

namespace PCSetupHub.Data.Models.Hardware
{
	public class PowerSupply : HardwareComponent
	{
		[Required(ErrorMessage = "Type is required.")]
		[StringLength(64, MinimumLength = 3,
			ErrorMessage = "Type must be between 3 and 64 characters long.")]
		[RegularExpression(@"^[A-Za-z/ ]+$",
			ErrorMessage = "Type can contain Latin letters, spaces and slashes (/).")]
		public string Type { get; set; } = string.Empty;

		[StringLength(64, MinimumLength = 3,
			ErrorMessage = "Efficiency must be between 3 and 64 characters long.")]
		[RegularExpression(@"^[A-Za-z/ ]+$",
			ErrorMessage = "Efficiency can contain Latin letters, spaces and slashes (/).")]
		public string? Efficiency { get; set; }

		[Range(200, int.MaxValue, ErrorMessage = "Wattage must be at least 200 W.")]
		public int Wattage { get; set; }

		[Required(ErrorMessage = "Modular is required.")]
		[StringLength(64, MinimumLength = 3,
			ErrorMessage = "Modular must be between 3 and 64 characters long.")]
		[RegularExpression(@"^[A-Za-z/ ]+$",
			ErrorMessage = "Modular can contain Latin letters, spaces and slashes (/).")]
		public string Modular { get; set; } = string.Empty;
		public ICollection<PcConfiguration>? PcConfigurations { get; private set; }
		public ICollection<ColorPowerSupply>? ColorPowerSupplies { get; private set; }
		public override string DisplayName => $"{Name} {Wattage} W";

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

		public void SetColorPowerSupplies(ICollection<ColorPowerSupply> colorPowerSupplies)
			=> ColorPowerSupplies = colorPowerSupplies;
	}
}