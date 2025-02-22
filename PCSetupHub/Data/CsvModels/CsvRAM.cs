using PCSetupHub.Models.Hardware;

namespace PCSetupHub.Data.CsvModels
{
	public class CsvRAM : RAM
	{
		public string Speed { get; set; } = string.Empty;
		public string Modules { get; set; } = string.Empty;

		public CsvRAM() { }
		public CsvRAM(string speed, string modules)
		{
			Speed = speed;
			Modules = modules;
		}
	}
}