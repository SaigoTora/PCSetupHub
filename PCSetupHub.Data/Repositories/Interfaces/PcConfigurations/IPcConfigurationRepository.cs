using PCSetupHub.Data.Models.Hardware;
using PCSetupHub.Data.Repositories.Base;

namespace PCSetupHub.Data.Repositories.Interfaces.PcConfigurations
{
	public interface IPcConfigurationRepository : IRepository<PcConfiguration>
	{
		/// <summary>
		/// Retrieves a PC configuration by its ID, optionally including all related components.
		/// </summary>
		/// <param name="id">The ID of the configuration to retrieve.</param>
		/// <param name="includeComponents">Whether to include related hardware components.</param>
		/// <param name="asNoTracking">Whether to disable entity tracking.</param>
		/// <returns>The matching PC configuration if found; otherwise, null.</returns>
		public Task<PcConfiguration?> GetByIdAsync(int id, bool includeComponents,
			bool asNoTracking = true);

		/// <summary>
		/// Retrieves a PC configuration by its ID, with optional includes and tracking settings.
		/// </summary>
		/// <param name="id">The ID of the configuration to retrieve.</param>
		/// <param name="includes">The related entities to include.</param>
		/// <param name="asNoTracking">Whether to disable entity tracking.</param>
		/// <returns>The matching PC configuration if found; otherwise, null.</returns>
		public Task<PcConfiguration?> GetByIdAsync(int id, PcConfigurationIncludes includes,
			bool asNoTracking = true);
	}
}