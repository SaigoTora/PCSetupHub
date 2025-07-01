using Microsoft.EntityFrameworkCore;

using PCSetupHub.Data.Models.Hardware;

namespace PCSetupHub.Data.Repositories.Implementations.PcConfigurations
{
	public class PowerSupplyRepository(PcSetupContext context)
		: HardwareComponentRepository<PowerSupply>(context)
	{
		protected override IQueryable<PowerSupply> GetQuery()
		{
			return Context.PowerSupplies
				.Include(ps => ps.ColorPowerSupplies!)
				.ThenInclude(cps => cps.Color);
		}
	}
}