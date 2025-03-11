using PCSetupHub.Data.Models.Hardware;

namespace PCSetupHub.Data.CsvModels
{
	public class CsvRam : Ram, ICsvConvertible<Ram>, ICsvColorModel
	{
		public string Speed { get; set; } = string.Empty;
		public string Modules { get; set; } = string.Empty;
		public string? Color { get; set; } = string.Empty;

		public Ram ConvertToModel()
		{
			(byte memoryType, int frequency) = GetByteInt(this.Speed);
			(byte modulesCount, int moduleCapacity) = GetByteInt(this.Modules);

			return new Ram(Name, Price, IsDefault, memoryType, frequency,
				modulesCount, moduleCapacity, FirstWordLatency, CASLatency);
		}
		private static (byte, int) GetByteInt(string str)
		{
			const char SEPARATOR = ',';
			var parts = str.Split(SEPARATOR);
			if (parts.Length == 0 || parts.Length > 2)
				return (0, 0);

			(byte number, bool valid) first = (default, default);
			first.valid = byte.TryParse(parts[0], out first.number);
			if (parts.Length == 1)
				return (0, int.Parse(parts[0]));

			(int number, bool valid) second = (default, default);
			second.valid = int.TryParse(parts[1], out second.number);

			if (!first.valid && !second.valid)
				return (0, 0);
			else if (!first.valid)
				return (0, second.number);
			else if (!second.valid)
				return (first.number, 0);

			return (first.number, second.number);
		}
	}
}