using CsvHelper;
using CsvHelper.Configuration;
using PCSetupHub.Data.CsvModels;
using PCSetupHub.Models.Hardware;
using System.Globalization;

namespace PCSetupHub.Data
{
	public static class DbInitializer
	{
		private static PcSetupContext? _context;
		private static CsvConfiguration? _csvConfig;

		public static void Initialize(PcSetupContext context)
		{
			context.Database.EnsureCreated();

			_context = context;
			_csvConfig = new CsvConfiguration(CultureInfo.InvariantCulture)
			{
				MissingFieldFound = null,
				HeaderValidated = null,
			};

			// Look for any users
			if (context.Users.Any())
				return;

			SeedDatabase();
		}

		#region Seed Database
		private static void SeedDatabase()
		{
			SeedStaticData();
			SeedPcConfigurationData();
			SeedUserData();
		}

		#region Seed static data
		private static void SeedStaticData()
		{
			SeedFriendshipStatuses();
			SeedHDDs();
			SeedMotherboards();
			SeedPowerSupplies();
			SeedProcessors();
			SeedRAMs();
			SeedSSDs();
			SeedVideoCards();
		}

		private static List<T> GetCsvData<T>(string fileName, string? dataFolderPath = null)
			where T : HardwareComponent
		{
			string path = dataFolderPath ?? Path.Combine(Environment.CurrentDirectory,
				"Data", "SeedData");
			string filePath = Path.Combine(path, fileName);

			if (!File.Exists(filePath))
				throw new FileNotFoundException($"The file '{fileName}' " +
					$"was not found at '{path}'.");

			using var reader = new StreamReader(filePath);
			using var csv = new CsvReader(reader, _csvConfig!);

			var records = csv.GetRecords<T>().ToList();
			records.ForEach(record => record.IsDefault = true);

			return records;
		}
		private static void SeedFriendshipStatuses()
		{

		}
		private static void SeedHDDs()
		{
			const string hddsCsvFileName = "hdds.csv";

			_context?.HDDs.AddRange(GetCsvData<HDD>(hddsCsvFileName));
			_context?.SaveChanges();
		}
		private static void SeedMotherboards()
		{
			const string motherboardsCsvFileName = "motherboards.csv";

			_context?.Motherboards.AddRange(GetCsvData<Motherboard>(motherboardsCsvFileName));
			_context?.SaveChanges();
		}
		private static void SeedPowerSupplies()
		{
			const string powerSuppliesCsvFileName = "power_supplies.csv";

			_context?.PowerSupplies.AddRange(GetCsvData<PowerSupply>(powerSuppliesCsvFileName));
			_context?.SaveChanges();
		}
		private static void SeedProcessors()
		{
			const string processorsCsvFileName = "processors.csv";

			_context?.Processors.AddRange(GetCsvData<Processor>(processorsCsvFileName));
			_context?.SaveChanges();
		}
		private static void SeedRAMs()
		{
			const string ramsCsvFileName = "rams.csv";

			var csvRams = GetCsvData<CsvRAM>(ramsCsvFileName);
			_context?.RAMs.AddRange(ConvertToRams(csvRams));
			_context?.SaveChanges();
		}
		private static void SeedSSDs()
		{
			const string ssdsCsvFileName = "ssds.csv";

			_context?.SSDs.AddRange(GetCsvData<SSD>(ssdsCsvFileName));
			_context?.SaveChanges();
		}
		private static void SeedVideoCards()
		{
			const string videoCardsCsvFileName = "video_cards.csv";

			_context?.VideoCards.AddRange(GetCsvData<VideoCard>(videoCardsCsvFileName));
			_context?.SaveChanges();
		}

		private static List<RAM> ConvertToRams(List<CsvRAM> csvRams)
		{
			List<RAM> rams = [];

			foreach (var csvRam in csvRams)
			{
				(byte memoryType, int frequency) = GetByteInt(csvRam.Speed);
				(byte modulesCount, int moduleCapacity) = GetByteInt(csvRam.Modules);
				rams.Add(new RAM(csvRam.Name, csvRam.IsDefault, memoryType, frequency,
					modulesCount, moduleCapacity, csvRam.Color, csvRam.FirstWordLatency,
					csvRam.CASLatency));
			}

			return rams;
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
		#endregion

		#region Seed pc configuration data
		private static void SeedPcConfigurationData()
		{
			SeedPcConfigurations();
			SeedPcConfigurationHDDs();
			SeedPcConfigurationRAMs();
			SeedPcConfigurationSSDs();
		}

		private static void SeedPcConfigurations()
		{

		}
		private static void SeedPcConfigurationHDDs()
		{

		}
		private static void SeedPcConfigurationRAMs()
		{

		}
		private static void SeedPcConfigurationSSDs()
		{

		}
		#endregion

		#region Seed user data
		private static void SeedUserData()
		{
			SeedUsers();
			SeedComments();
			SeedFriendships();
			SeedMessages();
		}

		private static void SeedUsers()
		{

		}
		private static void SeedComments()
		{

		}
		private static void SeedFriendships()
		{

		}
		private static void SeedMessages()
		{

		}
		#endregion
		#endregion
	}
}