

namespace PCSetupHub.Data
{
	public static class DbInitializer
	{
		public static void Initialize(PcSetupContext context)
		{
			context.Database.EnsureCreated();

			// Look for any users
			if (context.Users.Any())
				return;

			SeedData();
		}

		private static void SeedData()
		{
			SeedStaticData();
		}

		private static void SeedStaticData()
		{

		}
	}
}