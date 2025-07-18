﻿using Microsoft.AspNetCore.Mvc;

using PCSetupHub.Data.Models.Attributes;
using PCSetupHub.Data.Models.Hardware;
using PCSetupHub.Data.Models.Relationships;
using PCSetupHub.Data.Repositories.Base;
using PCSetupHub.Data.Repositories.Interfaces.PcConfigurations;
using PCSetupHub.Data.Repositories.Interfaces.Users;

namespace PCSetupHub.Web.Controllers.HardwareComponents
{
	[Route("[Controller]")]
	public class MotherboardController : HardwareSingleController<Motherboard>
	{
		private readonly IRepository<ColorMotherboard> ColorMotherboardRepository;

		protected override string ComponentName => "Motherboard";
		protected override bool IsComponentColorful => true;
		protected override PcConfigurationIncludes PcConfigurationIncludes =>
			PcConfigurationIncludes.Motherboard;

		public MotherboardController(ILogger<MotherboardController> logger,
			IPcConfigurationRepository pcConfigRepository,
			IHardwareComponentRepository<Motherboard> motherboardRepository,
			IRepository<Color> colorRepository,
			IRepository<ColorMotherboard> colorMotherboardRepository,
			IUserRepository userRepository)
			: base(logger, pcConfigRepository, motherboardRepository, colorRepository,
				  userRepository)
		{
			ColorMotherboardRepository = colorMotherboardRepository;
		}

		protected override Motherboard? GetComponent(PcConfiguration pcConfiguration)
			=> pcConfiguration.Motherboard;
		protected override void ChangeComponent(PcConfiguration pcConfiguration,
			Motherboard motherboard)
		{
			pcConfiguration.ChangeMotherboard(motherboard);
		}
		protected override void ClearComponent(PcConfiguration pcConfiguration)
			=> pcConfiguration.ClearMotherboard();
		protected override bool IsComponentInPcConfig(PcConfiguration pcConfig, int motherboardId)
			=> pcConfig.Motherboard?.Id == motherboardId;

		protected override async Task<Motherboard> UpdateColorRelationshipsAsync(
			Motherboard motherboard, List<int> colorIds)
		{
			try
			{
				// Getting existing relationships
				var existingRelations = await ColorMotherboardRepository
				.GetSomeAsync(cm => cm.MotherboardId == motherboard.Id);

				var existingColorIds = existingRelations.Select(r => r.ColorId).ToHashSet();
				var newColorIds = colorIds.ToHashSet();

				// Removing relationships that no longer exist
				var relationsToRemove = existingRelations
					.Where(r => !newColorIds.Contains(r.ColorId))
					.ToList();

				foreach (var relation in relationsToRemove)
					await ColorMotherboardRepository.DeleteAsync(relation);

				// Adding new relationships
				var relationsToAdd = newColorIds
					.Except(existingColorIds)
					.Select(colorId => new ColorMotherboard(colorId, motherboard.Id))
					.ToList();

				await ColorMotherboardRepository.AddAsync(relationsToAdd);

				// Updating the collection in entity
				var updatedRelations = existingRelations
					.Where(r => newColorIds.Contains(r.ColorId))
					.Concat(relationsToAdd)
					.ToList();

				if (relationsToAdd.Count != 0 || relationsToRemove.Count != 0)
				{
					motherboard.SetColorMotherboards(updatedRelations);
					Logger.LogInformation("Updated colors for Motherboard with id " +
						"{MotherboardId}: added {AddedCount}, removed {RemovedCount}",
						motherboard.Id, relationsToAdd.Count, relationsToRemove.Count);
				}

				return motherboard;
			}
			catch (Exception ex)
			{
				Logger.LogError(ex, "Failed to update colors for Motherboard with id " +
					"{MotherboardId}", motherboard.Id);
				throw;
			}
		}
	}
}