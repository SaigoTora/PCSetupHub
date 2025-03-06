using PCSetupHub.Data.Models.Hardware;

namespace PCSetupHub.Data.CsvModels
{
	public class CsvHDD : HDD, ICsvConvertible<HDD>, ICsvColorModel
	{
		public string? Color { get; set; } = string.Empty;

		public HDD ConvertToModel()
			=> new(Name, Price, IsDefault, Type, Interface, Capacity);
	}
}