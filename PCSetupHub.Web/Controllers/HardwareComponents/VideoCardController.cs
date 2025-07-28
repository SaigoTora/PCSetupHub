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
	public class VideoCardController : HardwareSingleController<VideoCard>
	{
		private readonly IRepository<ColorVideoCard> ColorVideoCardRepository;

		protected override string ComponentName => "VideoCard";
		protected override bool IsComponentColorful => true;
		protected override PcConfigurationIncludes PcConfigurationIncludes =>
			PcConfigurationIncludes.VideoCard;

		public VideoCardController(ILogger<VideoCardController> logger,
			IPcConfigurationRepository pcConfigRepository,
			IHardwareComponentRepository<VideoCard> videoCardRepository,
			IRepository<Color> colorRepository,
			IRepository<ColorVideoCard> colorVideoCardRepository, IUserRepository userRepository)
			: base(logger, pcConfigRepository, videoCardRepository, colorRepository,
				  userRepository)
		{
			ColorVideoCardRepository = colorVideoCardRepository;
		}

		protected override VideoCard? GetComponent(PcConfiguration pcConfiguration)
			=> pcConfiguration.VideoCard;
		protected override void ChangeComponent(PcConfiguration pcConfiguration,
			VideoCard videoCard)
		{
			pcConfiguration.VideoCard = videoCard;
		}
		protected override void ClearComponent(PcConfiguration pcConfiguration)
			=> pcConfiguration.VideoCard = null;
		protected override bool IsComponentInPcConfig(PcConfiguration pcConfig, int videoCardId)
			=> pcConfig.VideoCard?.Id == videoCardId;

		protected override async Task<VideoCard> UpdateColorRelationshipsAsync(VideoCard videoCard,
			List<int> colorIds)
		{
			try
			{
				// Getting existing relationships
				var existingRelations = await ColorVideoCardRepository
					.GetSomeAsync(cvc => cvc.VideoCardId == videoCard.Id);

				var existingColorIds = existingRelations.Select(r => r.ColorId).ToHashSet();
				var newColorIds = colorIds.ToHashSet();

				// Removing relationships that no longer exist
				var relationsToRemove = existingRelations
					.Where(r => !newColorIds.Contains(r.ColorId))
					.ToList();

				foreach (var relation in relationsToRemove)
					await ColorVideoCardRepository.DeleteAsync(relation);

				// Adding new relationships
				var relationsToAdd = newColorIds
					.Except(existingColorIds)
					.Select(colorId => new ColorVideoCard(colorId, videoCard.Id))
					.ToList();

				await ColorVideoCardRepository.AddAsync(relationsToAdd);

				// Updating the collection in entity
				var updatedRelations = existingRelations
					.Where(r => newColorIds.Contains(r.ColorId))
					.Concat(relationsToAdd)
					.ToList();

				if (relationsToAdd.Count != 0 || relationsToRemove.Count != 0)
				{
					videoCard.ColorVideoCards = updatedRelations;
					Logger.LogInformation("Updated colors for VideoCard with id {VideoCardId}: " +
						"added {AddedCount}, removed {RemovedCount}", videoCard.Id,
						relationsToAdd.Count, relationsToRemove.Count);
				}

				return videoCard;
			}
			catch (Exception ex)
			{
				Logger.LogError(ex, "Failed to update colors for VideoCard with id {VideoCardId}",
					videoCard.Id);
				throw;
			}
		}
	}
}