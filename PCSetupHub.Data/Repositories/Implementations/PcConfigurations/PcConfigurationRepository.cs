using Microsoft.EntityFrameworkCore;

using PCSetupHub.Data.Models.Hardware;
using PCSetupHub.Data.Repositories.Base;
using PCSetupHub.Data.Repositories.Interfaces.PcConfigurations;

namespace PCSetupHub.Data.Repositories.Implementations.PcConfigurations
{
	public class PcConfigurationRepository(PcSetupContext context)
		: BaseRepo<PcConfiguration>(context), IPcConfigurationRepository
	{
		public async Task<PcConfiguration?> GetByIdAsync(int id)
		{
			return await Context.PcConfigurations.AsQueryable()
				.Include(pcConfig => pcConfig.Type)
				.Include(pcConfig => pcConfig.Processor)
				.Include(pcConfig => pcConfig.User)
				.FirstOrDefaultAsync(pcConfig => pcConfig.Id == id);
		}
	}
}