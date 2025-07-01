using Microsoft.EntityFrameworkCore;

using PCSetupHub.Data.Models.Hardware;

namespace PCSetupHub.Data.Repositories.Implementations.PcConfigurations
{
	public class RamRepository(PcSetupContext context)
		: HardwareComponentRepository<Ram>(context)
	{
		protected override IQueryable<Ram> GetQuery()
		{
			return Context.Rams
				.Include(ram => ram.ColorRams!)
				.ThenInclude(cRam => cRam.Color);
		}
	}
}