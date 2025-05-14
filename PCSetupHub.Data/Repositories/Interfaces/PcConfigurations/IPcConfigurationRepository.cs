using PCSetupHub.Data.Models.Hardware;

namespace PCSetupHub.Data.Repositories.Interfaces.PcConfigurations
{
	public interface IPcConfigurationRepository
	{
		public Task<PcConfiguration?> GetByIdAsync(int id);
	}
}