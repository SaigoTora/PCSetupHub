using PCSetupHub.Models.Hardware;

namespace PCSetupHub.Data.CsvModels
{
	public class CsvPowerSupply : PowerSupply, ICsvConvertible<PowerSupply>, ICsvColorModel
	{
		public string? Color { get; set; } = string.Empty;

		public PowerSupply ConvertToModel()
			=> new(Name, Price, IsDefault, Type, Efficiency, Wattage, Modular);
	}
}