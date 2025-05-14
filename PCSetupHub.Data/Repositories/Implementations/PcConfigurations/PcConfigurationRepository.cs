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
				.Include(pcConfig => pcConfig.VideoCard)
					.ThenInclude(vc => vc!.ColorVideoCards!).ThenInclude(x => x.Color)
				.Include(pcConfig => pcConfig.Motherboard)
					.ThenInclude(m => m!.ColorMotherboards!).ThenInclude(x => x.Color)
				.Include(pcConfig => pcConfig.PowerSupply)
					.ThenInclude(ps => ps!.ColorPowerSupplies!).ThenInclude(x => x.Color)
				.Include(pcConfig => pcConfig!.PcConfigurationSsds!).ThenInclude(pcSsd => pcSsd.Ssd)
				.Include(pcConfig => pcConfig!.PcConfigurationRams!).ThenInclude(pcRam => pcRam.Ram)
					.ThenInclude(ram => ram!.ColorRams!).ThenInclude(x => x.Color)
				.Include(pcConfig => pcConfig!.PcConfigurationHdds!).ThenInclude(pcHdd => pcHdd.Hdd)
					.ThenInclude(hdd => hdd!.ColorHdds!).ThenInclude(x => x.Color)
				.Include(pcConfig => pcConfig.User)
				.FirstOrDefaultAsync(pcConfig => pcConfig.Id == id);
		}
	}
}