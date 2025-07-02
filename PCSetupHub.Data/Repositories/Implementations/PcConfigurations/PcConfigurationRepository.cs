using Microsoft.EntityFrameworkCore;

using PCSetupHub.Data.Models.Hardware;
using PCSetupHub.Data.Repositories.Base;
using PCSetupHub.Data.Repositories.Interfaces.PcConfigurations;

namespace PCSetupHub.Data.Repositories.Implementations.PcConfigurations
{
	public class PcConfigurationRepository(PcSetupContext context)
		: BaseRepo<PcConfiguration>(context), IPcConfigurationRepository
	{
		public async Task<PcConfiguration?> GetByIdAsync(int id, bool includeComponents)
		{
			if (includeComponents)
			{
				return await GetByIdAsync(id,
					PcConfigurationIncludes.Processor |
					PcConfigurationIncludes.VideoCard |
					PcConfigurationIncludes.Motherboard |
					PcConfigurationIncludes.PowerSupply |
					PcConfigurationIncludes.Ssds |
					PcConfigurationIncludes.Rams |
					PcConfigurationIncludes.Hdds);
			}

			return await GetByIdAsync(id, PcConfigurationIncludes.None);
		}

		public async Task<PcConfiguration?> GetByIdAsync(int id, PcConfigurationIncludes includes,
			bool asNoTracking = false)
		{
			var query = Context.PcConfigurations
				.Include(pc => pc.Type)
				.Include(pc => pc.User)
				.AsQueryable();

			ApplyProcessorInclude(ref query, includes);
			ApplyVideoCardInclude(ref query, includes);
			ApplyMotherboardInclude(ref query, includes);
			ApplyPowerSupplyInclude(ref query, includes);
			ApplySsdInclude(ref query, includes);
			ApplyRamInclude(ref query, includes);
			ApplyHddInclude(ref query, includes);

			if (asNoTracking)
				query = query.AsNoTracking();

			return await query.AsSplitQuery().FirstOrDefaultAsync(pc => pc.Id == id);
		}

		private static void ApplyProcessorInclude(ref IQueryable<PcConfiguration> query,
			PcConfigurationIncludes includes)
		{
			if (includes.HasFlag(PcConfigurationIncludes.Processor))
				query = query.Include(pc => pc.Processor);
		}
		private static void ApplyVideoCardInclude(ref IQueryable<PcConfiguration> query,
			PcConfigurationIncludes includes)
		{
			if (includes.HasFlag(PcConfigurationIncludes.VideoCard))
			{
				query = query
					.Include(pc => pc.VideoCard)
						.ThenInclude(vc => vc!.ColorVideoCards!)
						.ThenInclude(x => x.Color);
			}
		}
		private static void ApplyMotherboardInclude(ref IQueryable<PcConfiguration> query,
			PcConfigurationIncludes includes)
		{
			if (includes.HasFlag(PcConfigurationIncludes.Motherboard))
			{
				query = query
					.Include(pc => pc.Motherboard)
						.ThenInclude(m => m!.ColorMotherboards!)
						.ThenInclude(x => x.Color);
			}
		}
		private static void ApplyPowerSupplyInclude(ref IQueryable<PcConfiguration> query,
			PcConfigurationIncludes includes)
		{
			if (includes.HasFlag(PcConfigurationIncludes.PowerSupply))
			{
				query = query
					.Include(pc => pc.PowerSupply)
						.ThenInclude(ps => ps!.ColorPowerSupplies!)
						.ThenInclude(x => x.Color);
			}
		}
		private static void ApplySsdInclude(ref IQueryable<PcConfiguration> query,
			PcConfigurationIncludes includes)
		{
			if (includes.HasFlag(PcConfigurationIncludes.Ssds))
			{
				query = query
					.Include(pc => pc.PcConfigurationSsds!)
						.ThenInclude(pcSsd => pcSsd.Ssd);
			}
		}
		private static void ApplyRamInclude(ref IQueryable<PcConfiguration> query,
			PcConfigurationIncludes includes)
		{
			if (includes.HasFlag(PcConfigurationIncludes.Rams))
			{
				query = query
					.Include(pc => pc.PcConfigurationRams!)
						.ThenInclude(pcRam => pcRam.Ram)
						.ThenInclude(ram => ram!.ColorRams!)
						.ThenInclude(x => x.Color);
			}
		}
		private static void ApplyHddInclude(ref IQueryable<PcConfiguration> query,
			PcConfigurationIncludes includes)
		{
			if (includes.HasFlag(PcConfigurationIncludes.Hdds))
			{
				query = query
					.Include(pc => pc.PcConfigurationHdds!)
						.ThenInclude(pcHdd => pcHdd.Hdd)
						.ThenInclude(hdd => hdd!.ColorHdds!)
						.ThenInclude(x => x.Color);
			}
		}
	}
}