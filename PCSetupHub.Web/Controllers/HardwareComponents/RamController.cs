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
	public class RamController : HardwareMultiController<Ram>
	{
		private readonly IRepository<ColorRam> ColorRamRepository;

		protected override string ComponentName => "Ram";
		protected override bool IsComponentColorful => true;
		protected override int MaxAllowedCount => PcConfiguration.MAX_RAM_COUNT;

		private readonly IRepository<PcConfigurationRam> _pcConfigRamRepository;

		public RamController(IHardwareComponentRepository<Ram> ramRepository,
			IRepository<PcConfigurationRam> pcConfigRamRepository,
			IRepository<Color> colorRepository,
			IRepository<ColorRam> colorRamRepository,
			IUserRepository userRepository)
			: base(ramRepository, colorRepository, userRepository)
		{
			_pcConfigRamRepository = pcConfigRamRepository;
			ColorRamRepository = colorRamRepository;
		}

		protected override async Task<List<int>> GetRelatedComponentIdsAsync(int pcConfigId)
		{
			return [.. (await _pcConfigRamRepository
				.GetSomeAsync(r => r.PcConfigurationId == pcConfigId))
				.Select(r => r.RamId)];
		}
		protected override async Task UpdateRelationAsync(int pcConfigId, int currentId, int newId)
		{
			var item = (await _pcConfigRamRepository
				.GetSomeAsync(r => r.PcConfigurationId == pcConfigId && r.RamId == currentId))
				.FirstOrDefault();

			if (item == null)
				return;

			item.ChangeRamId(newId);
			await _pcConfigRamRepository.UpdateAsync(item);
		}
		protected override async Task ClearRelationAsync(int pcConfigId, int ramId)
		{
			var item = (await _pcConfigRamRepository
				.GetSomeAsync(r => r.PcConfigurationId == pcConfigId && r.RamId == ramId))
				.FirstOrDefault();

			if (item == null)
				return;

			await _pcConfigRamRepository.DeleteAsync(item);
		}
		protected override async Task CreateRelationAsync(int pcConfigId, int ramId)
		{
			PcConfigurationRam newItem = new(pcConfigId, ramId);
			await _pcConfigRamRepository.AddAsync(newItem);
		}
		protected override async Task<Ram> UpdateColorRelationshipsAsync(
			Ram ram, List<int> colorIds)
		{
			// Getting existing relationships
			var existingRelations = await ColorRamRepository
				.GetSomeAsync(cm => cm.RamId == ram.Id);

			var existingColorIds = existingRelations.Select(r => r.ColorId).ToHashSet();
			var newColorIds = colorIds.ToHashSet();

			// Removing relationships that no longer exist
			var relationsToRemove = existingRelations
				.Where(r => !newColorIds.Contains(r.ColorId))
				.ToList();

			foreach (var relation in relationsToRemove)
				await ColorRamRepository.DeleteAsync(relation);

			// Adding new relationships
			var relationsToAdd = newColorIds
				.Except(existingColorIds)
				.Select(colorId => new ColorRam(colorId, ram.Id))
				.ToList();

			await ColorRamRepository.AddAsync(relationsToAdd);

			// Updating the collection in entity
			var updatedRelations = existingRelations
				.Where(r => newColorIds.Contains(r.ColorId))
				.Concat(relationsToAdd)
				.ToList();

			ram.SetColorRams(updatedRelations);

			return ram;
		}
	}
}