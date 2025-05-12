using PCSetupHub.Data.Models.Hardware;

namespace PCSetupHub.Data.CsvModels
{
	public class CsvVideoCard : VideoCard, ICsvConvertible<VideoCard>, ICsvColorModel
	{
		public string? Color { get; set; } = string.Empty;

		public VideoCard ConvertToModel()
			=> new(Name, Price, IsDefault, Chipset, Memory,
				CoreClock, BoostClock, Length);
	}
}