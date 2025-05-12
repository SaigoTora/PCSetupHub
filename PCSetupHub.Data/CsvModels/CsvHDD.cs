using PCSetupHub.Data.Models.Hardware;

namespace PCSetupHub.Data.CsvModels
{
	public class CsvHdd : Hdd, ICsvConvertible<Hdd>, ICsvColorModel
	{
		public string? Color { get; set; } = string.Empty;

		public Hdd ConvertToModel()
			=> new(Name, Price, IsDefault, Type, Interface, Capacity);
	}
}