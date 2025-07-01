using Microsoft.EntityFrameworkCore;

using PCSetupHub.Data.Models.Hardware;

namespace PCSetupHub.Data.Repositories.Implementations.PcConfigurations
{
	public class MotherboardRepository(PcSetupContext context)
	: HardwareComponentRepository<Motherboard>(context)
	{
		protected override IQueryable<Motherboard> GetQuery()
		{
			return Context.Motherboards
				.Include(m => m.ColorMotherboards!)
				.ThenInclude(cm => cm.Color);
		}
	}
}