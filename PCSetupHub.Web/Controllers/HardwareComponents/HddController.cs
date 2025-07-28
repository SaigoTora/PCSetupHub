using Microsoft.AspNetCore.Mvc;

using PCSetupHub.Data.Models.Attributes;
using PCSetupHub.Data.Models.Hardware;
using PCSetupHub.Data.Models.Relationships;
using PCSetupHub.Data.Repositories.Base;
using PCSetupHub.Data.Repositories.Interfaces.PcConfigurations;
using PCSetupHub.Data.Repositories.Interfaces.Users;

namespace PCSetupHub.Web.Controllers.HardwareComponents
{
	[Route("[Controller]")]
	public class HddController : HardwareMultiController<Hdd>
	{
		private readonly IRepository<ColorHdd> ColorHddRepository;

		protected override string ComponentName => "Hdd";
		protected override bool IsComponentColorful => true;
		protected override int MaxAllowedCount => PcConfiguration.MAX_HDD_COUNT;

		private readonly IRepository<PcConfigurationHdd> _pcConfigHddRepository;

		public HddController(ILogger<HddController> logger,
			IHardwareComponentRepository<Hdd> hddRepository,
			IRepository<PcConfigurationHdd> pcConfigHddRepository,
			IRepository<Color> colorRepository,
			IRepository<ColorHdd> colorHddRepository,
			IUserRepository userRepository)
			: base(logger, hddRepository, colorRepository, userRepository)
		{
			_pcConfigHddRepository = pcConfigHddRepository;
			ColorHddRepository = colorHddRepository;
		}

		protected override async Task<List<int>> GetRelatedComponentIdsAsync(int pcConfigId)
		{
			return [.. (await _pcConfigHddRepository
				.GetSomeAsync(r => r.PcConfigurationId == pcConfigId))
				.Select(r => r.HddId)];
		}
		protected override async Task UpdateRelationAsync(int pcConfigId, int currentId, int newId)
		{
			var item = (await _pcConfigHddRepository
				.GetSomeAsync(r => r.PcConfigurationId == pcConfigId && r.HddId == currentId))
				.FirstOrDefault();

			if (item == null)
				return;

			item.HddId = newId;
			await _pcConfigHddRepository.UpdateAsync(item);
		}
		protected override async Task ClearRelationAsync(int pcConfigId, int hddId)
		{
			var item = (await _pcConfigHddRepository
				.GetSomeAsync(r => r.PcConfigurationId == pcConfigId && r.HddId == hddId))
				.FirstOrDefault();

			if (item == null)
				return;

			await _pcConfigHddRepository.DeleteAsync(item);
		}
		protected override async Task CreateRelationAsync(int pcConfigId, int hddId)
		{
			PcConfigurationHdd newItem = new(pcConfigId, hddId);
			await _pcConfigHddRepository.AddAsync(newItem);
		}
		protected override async Task<Hdd> UpdateColorRelationshipsAsync(
			Hdd hdd, List<int> colorIds)
		{
			try
			{
				// Getting existing relationships
				var existingRelations = await ColorHddRepository
				.GetSomeAsync(cm => cm.HddId == hdd.Id);

				var existingColorIds = existingRelations.Select(r => r.ColorId).ToHashSet();
				var newColorIds = colorIds.ToHashSet();

				// Removing relationships that no longer exist
				var relationsToRemove = existingRelations
					.Where(r => !newColorIds.Contains(r.ColorId))
					.ToList();

				foreach (var relation in relationsToRemove)
					await ColorHddRepository.DeleteAsync(relation);

				// Adding new relationships
				var relationsToAdd = newColorIds
					.Except(existingColorIds)
					.Select(colorId => new ColorHdd(colorId, hdd.Id))
					.ToList();

				await ColorHddRepository.AddAsync(relationsToAdd);

				// Updating the collection in entity
				var updatedRelations = existingRelations
					.Where(r => newColorIds.Contains(r.ColorId))
					.Concat(relationsToAdd)
					.ToList();

				if (relationsToAdd.Count != 0 || relationsToRemove.Count != 0)
				{
					hdd.ColorHdds = updatedRelations;
					Logger.LogInformation("Updated colors for Hdd with id {HddId}: " +
						"added {AddedCount}, removed {RemovedCount}", hdd.Id,
						relationsToAdd.Count, relationsToRemove.Count);
				}

				return hdd;
			}
			catch (Exception ex)
			{
				Logger.LogError(ex, "Failed to update colors for Hdd with id {HddId}", hdd.Id);
				throw;
			}
		}
	}
}