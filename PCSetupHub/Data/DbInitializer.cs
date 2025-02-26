using CsvHelper;
using CsvHelper.Configuration;
using System.Globalization;

using PCSetupHub.Data.CsvModels;
using PCSetupHub.Models.Hardware;
using PCSetupHub.Models.Relationships;
using PCSetupHub.Models.Users;

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
			_context?.FriendshipStatuses.AddRange(
				new FriendshipStatus("Pending"),
				new FriendshipStatus("Accepted"),
				new FriendshipStatus("Cancelled")
			);

			_context?.SaveChanges();
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
				rams.Add(new RAM(csvRam.Name, csvRam.Price, csvRam.IsDefault, memoryType, frequency,
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
			_context?.PcConfigurations.AddRange(
				new PcConfiguration(null, null, null, null),
				new PcConfiguration(null, null, null, null),
				new PcConfiguration(54, 940, null, null),
				new PcConfiguration(32, 276, 505, 280),
				new PcConfiguration(41, 24, 725, 1651));

			_context?.SaveChanges();
		}
		private static void SeedPcConfigurationHDDs()
		{
			_context?.PcConfigurationHDDs.AddRange(
				new PcConfigurationHDD(3, 219),
				new PcConfigurationHDD(3, 334),
				new PcConfigurationHDD(5, 214));

			_context?.SaveChanges();
		}
		private static void SeedPcConfigurationRAMs()
		{
			_context?.PcConfigurationRAMs.AddRange(
				new PcConfigurationRAM(2, 5251),
				new PcConfigurationRAM(3, 1136),
				new PcConfigurationRAM(3, 3879),
				new PcConfigurationRAM(5, 2165));

			_context?.SaveChanges();
		}
		private static void SeedPcConfigurationSSDs()
		{
			_context?.PcConfigurationSSDs.AddRange(
				new PcConfigurationSSD(2, 3965),
				new PcConfigurationSSD(2, 2673),
				new PcConfigurationSSD(3, 850),
				new PcConfigurationSSD(5, 1755));

			_context?.SaveChanges();
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
			_context?.Users.AddRange(
				new User("max_power", "StrongPass567!", "Maxim", "max_power@gmail.com", 1),
				new User("anna_dev", "AnnaPass123!", "Anna", "anna_dev@gmail.com", 2),
				new User("alex_gamer", "Pass1234!", "Alexander", "alex_gamer@gmail.com", 3),
				new User("kate_player", "KateGamer456!", "Kate", "kate_player@gmail.com", 4),
				new User("niko_coder", "Secure789!", "Nikolay", "niko_coder@gmail.com", 5));

			_context?.SaveChanges();
		}
		private static void SeedComments()
		{
			_context?.Comments.AddRange(
				new Comment(4, 2, "Cool profile, but a little lacking in information about your PC."),
				new Comment(5, 3, "I had a similar build once, more to come!" +
				"\nPlease add the motherboard and power supply so we can fully evaluate your build."),
				new Comment(2, 4, "Not bad, but I think the video card should be improved."),
				new Comment(4, 4, "Yes, I agree. I'm going to upgrade the video card this year when " +
				"I save up some money. =)"),
				new Comment(3, 4, "This is another level! Incredible components!"),
				new Comment(2, 5, "Excellent build!"),
				new Comment(4, 5, "Great combination! But I think the power supply will need to be updated soon!"),
				new Comment(2, 5, "I dream of a similar build!"));
		}
		private static void SeedFriendships()
		{
			_context?.Friendships.AddRange(
				new Friendship(2, 3, 2),
				new Friendship(2, 4, 1),
				new Friendship(2, 5, 3),
				new Friendship(3, 4, 2),
				new Friendship(3, 5, 1),
				new Friendship(4, 5, 2));

			_context?.SaveChanges();
		}
		private static void SeedMessages()
		{
			_context?.Messages.AddRange(
				new Message(1, 3, "Hi, I think we could be friends, what do you think?", true),
				new Message(1, 5, "Hi! I think we could be friends, what do you think?", false),
				new Message(2, 3, "Hello, I'm thinking about upgrading my PC.", true),
				new Message(2, 3, "Any recommendations?", true),
				new Message(2, 4, "Good evening, you have a cool profile!", true),
				new Message(2, 4, "Add me as your friend.", false),
				new Message(3, 4, "Good morning, let's meet at our place today.", true),
				new Message(4, 3, "Sorry, I'm busy today. =(\nLet's do it another time.", false),
				new Message(4, 5, "Hey, how's it going?", true),
				new Message(5, 4, "Hey! Pretty good, just finished work.", true),
				new Message(5, 4, "You?", true),
				new Message(4, 5, "Same here. Today I'll go to a cafe with my family.", true));

			_context?.SaveChanges();
		}
		#endregion
		#endregion
	}
}