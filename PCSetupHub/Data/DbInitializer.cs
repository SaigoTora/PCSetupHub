using CsvHelper;
using CsvHelper.Configuration;
using Microsoft.EntityFrameworkCore;
using System.Globalization;

using PCSetupHub.Data.CsvModels;
using PCSetupHub.Models.Attributes;
using PCSetupHub.Models.Base;
using PCSetupHub.Models.Hardware;
using PCSetupHub.Models.Relationships;
using PCSetupHub.Models.Users;

namespace PCSetupHub.Data
{
	public static class DbInitializer
	{
		private static PcSetupContext? _context;
		private static CsvConfiguration? _csvConfig;

		private static readonly Dictionary<string, Color> existingColors = [];

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

		private static T[] GetCsvData<T>(string fileName, string? dataFolderPath = null)
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

			var records = csv.GetRecords<T>().ToArray();
			for (int i = 0; i < records.Length; i++)
				records[i].IsDefault = true;

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
			var csvHdds = GetCsvData<CsvHDD>(hddsCsvFileName);

			SeedColoredHardware(csvHdds, _context!.HDDs, _context.ColorHDDs,
				(colorID, hddID) => new ColorHDD(colorID, hddID));
		}
		private static void SeedMotherboards()
		{
			const string motherboardsCsvFileName = "motherboards.csv";
			var csvMotherboards = GetCsvData<CsvMotherboard>(motherboardsCsvFileName);

			SeedColoredHardware(csvMotherboards, _context!.Motherboards,
				_context.ColorMotherboards,
				(colorID, motherboardId) => new ColorMotherboard(colorID, motherboardId));
		}
		private static void SeedPowerSupplies()
		{
			const string powerSuppliesCsvFileName = "power_supplies.csv";
			var csvPowerSupplies = GetCsvData<CsvPowerSupply>(powerSuppliesCsvFileName);

			SeedColoredHardware(csvPowerSupplies, _context!.PowerSupplies,
				_context.ColorPowerSupplies,
				(colorID, powerSupplyID) => new ColorPowerSupply(colorID, powerSupplyID));
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

			SeedColoredHardware(csvRams, _context!.RAMs, _context.ColorRams,
				(colorID, ramID) => new ColorRAM(colorID, ramID));
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
			var csvVideoCards = GetCsvData<CsvVideoCard>(videoCardsCsvFileName);

			SeedColoredHardware(csvVideoCards, _context!.VideoCards, _context.ColorVideoCards,
				(colorID, videoCardID) => new ColorVideoCard(colorID, videoCardID));
		}

		private static void SeedColoredHardware<T1, T2, T3>(T1[] csvModels,
			DbSet<T2> dbModelSet, DbSet<T3> dbColorRelationSet,
			Func<int, int, T3> createColorRelation)
			where T1 : ICsvConvertible<T2>, ICsvColorModel
			where T2 : class
			where T3 : class
		{
			T2[] newModels = csvModels.Select(csvModel => csvModel.ConvertToModel()).ToArray();
			dbModelSet.AddRange(newModels);
			_context?.SaveChanges();

			AddUniqueColorsToContext(csvModels);
			AddNewColorRelationships(csvModels, newModels, dbColorRelationSet,
				createColorRelation);
		}
		private static void AddNewColorRelationships<T1, T2, T3>(T1[] csvModels,
			T2[] newModels, DbSet<T3> dbSet, Func<int, int, T3> createColorRelation)
			where T1 : ICsvColorModel
			where T3 : class
		{
			List<T3> colorRelationTable = [];

			for (int i = 0; i < csvModels.Length; i++)
			{
				if (newModels[i] is not BaseEntity model)
					continue;

				string[] colors = ParseColors(csvModels[i].Color);
				foreach (var colorName in colors)
					colorRelationTable.Add(createColorRelation(existingColors[colorName].Id,
						model.Id));
			}

			dbSet.AddRange(colorRelationTable);
			_context?.SaveChanges();
		}
		private static void AddUniqueColorsToContext<T>(T[] csvModels)
			where T : ICsvColorModel
		{
			List<Color> newColors = [];

			for (int i = 0; i < csvModels.Length; i++)
			{
				string[] colors = ParseColors(csvModels[i].Color);

				foreach (var colorName in colors)
					if (!existingColors.ContainsKey(colorName))
					{
						var newColor = new Color(colorName);
						newColors.Add(newColor);
						existingColors[colorName] = newColor;
					}
			}

			if (newColors.Count > 0)
			{
				_context?.Colors.AddRange(newColors);
				_context?.SaveChanges();
			}
		}
		private static string[] ParseColors(string? str)
		{
			const char SEPARATOR = '/';
			if (string.IsNullOrWhiteSpace(str))
				return [];

			string[] resultArr = str.Split(SEPARATOR);
			for (int i = 0; i < resultArr.Length; i++)
				resultArr[i] = resultArr[i].Trim(' ');

			return resultArr;
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
				new Comment(4, 2, "Cool profile, but a little lacking in " +
				"information about your PC."),
				new Comment(5, 3, "I had a similar build once, more to come!" +
				"\nPlease add the motherboard and power supply " +
				"so we can fully evaluate your build."),
				new Comment(2, 4, "Not bad, but I think the video card should be improved."),
				new Comment(4, 4, "Yes, I agree. I'm going to upgrade the video card " +
				"this year when I save up some money. =)"),
				new Comment(3, 4, "This is another level! Incredible components!"),
				new Comment(2, 5, "Excellent build!"),
				new Comment(4, 5, "Great combination! But I think the power supply will need " +
				"to be updated soon!"),
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