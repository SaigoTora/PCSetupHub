using Microsoft.EntityFrameworkCore;

using PCSetupHub.Data.Models.Hardware;

namespace PCSetupHub.Data.Repositories.Implementations.PcConfigurations
{
	public class HddRepository(PcSetupContext context)
	: HardwareComponentRepository<Hdd>(context)
	{
		protected override IQueryable<Hdd> GetQuery()
		{
			return Context.Hdds
				.Include(hdd => hdd.ColorHdds!)
				.ThenInclude(cHdd => cHdd.Color);
		}
	}
}