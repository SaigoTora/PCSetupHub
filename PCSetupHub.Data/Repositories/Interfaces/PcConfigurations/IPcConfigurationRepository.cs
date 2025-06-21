using PCSetupHub.Data.Models.Hardware;
using PCSetupHub.Data.Repositories.Base;

namespace PCSetupHub.Data.Repositories.Interfaces.PcConfigurations
{
	public interface IPcConfigurationRepository : IRepository<PcConfiguration>
	{
		public Task<PcConfiguration?> GetByIdAsync(int id, bool includeComponents);
		public Task<PcConfiguration?> GetByIdAsync(int id, PcConfigurationIncludes includes,
			bool asNoTracking = false);
	}
}