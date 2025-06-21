using Microsoft.AspNetCore.Mvc;

using PCSetupHub.Data.Models.Hardware;
using PCSetupHub.Data.Repositories.Base;
using PCSetupHub.Data.Repositories.Interfaces.PcConfigurations;
using PCSetupHub.Data.Repositories.Interfaces.Users;

namespace PCSetupHub.Web.Controllers.HardwareComponents
{
	[Route("[Controller]")]
	public class VideoCardController : HardwareBaseController<VideoCard>
	{
		protected override string ComponentName => "VideoCard";
		protected override PcConfigurationIncludes PcConfigurationIncludes =>
			PcConfigurationIncludes.VideoCard;

		public VideoCardController(IPcConfigurationRepository pcConfigRepository,
			IRepository<VideoCard> videoCardRepository, IUserRepository userRepository)
			: base(pcConfigRepository, videoCardRepository, userRepository)
		{ }

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
	}
}