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
	public class VideoCardController : HardwareBaseController<VideoCard>
	{
		private readonly IRepository<ColorVideoCard> ColorVideoCardRepository;

		protected override string ComponentName => "VideoCard";
		protected override PcConfigurationIncludes PcConfigurationIncludes =>
			PcConfigurationIncludes.VideoCard;

		public VideoCardController(IPcConfigurationRepository pcConfigRepository,
			IRepository<VideoCard> videoCardRepository, IRepository<Color> colorRepository,
			IRepository<ColorVideoCard> colorVideoCardRepository, IUserRepository userRepository)
			: base(pcConfigRepository, videoCardRepository, colorRepository, userRepository)
		{
			ColorVideoCardRepository = colorVideoCardRepository;
		}

		protected override VideoCard? GetComponent(PcConfiguration pcConfiguration)
			=> pcConfiguration.VideoCard;
		protected override void ChangeComponent(PcConfiguration pcConfiguration,
			VideoCard component)
		{
			pcConfiguration.ChangeVideoCard(component);
		}
		protected override void ClearComponent(PcConfiguration pcConfiguration)
			=> pcConfiguration.ClearVideoCard();
		protected override bool IsComponentInPcConfig(PcConfiguration pcConfig, int componentId)
			=> pcConfig.VideoCard?.Id == componentId;

		protected override async Task<VideoCard> AddColorRelationshipsAsync(VideoCard videoCard,
			List<int> colorIds)
		{
			// Deleting old relationships
			var oldColorVideoCards = await ColorVideoCardRepository
				.GetSomeAsync(cvc => cvc.VideoCardId == videoCard.Id);

			foreach (var oldItem in oldColorVideoCards)
				await ColorVideoCardRepository.DeleteAsync(oldItem);

			// Creating new relationships
			List<ColorVideoCard> newColorVideoCards = [];

			foreach (var colorId in colorIds)
				newColorVideoCards.Add(new(colorId, videoCard.Id));

			await ColorVideoCardRepository.AddAsync(newColorVideoCards);
			videoCard.SetColorVideoCards(newColorVideoCards);

			return videoCard;
		}
	}
}