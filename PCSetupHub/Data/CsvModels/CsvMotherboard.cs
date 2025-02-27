using PCSetupHub.Models.Hardware;

namespace PCSetupHub.Data.CsvModels
{
	public class CsvMotherboard : Motherboard, ICsvConvertible<Motherboard>, ICsvColorModel
	{
		public string? Color { get; set; } = string.Empty;

		public Motherboard ConvertToModel()
			=> new(Name, Price, IsDefault, Socket, FormFactor, MaxMemory, MemorySlots);
	}
}