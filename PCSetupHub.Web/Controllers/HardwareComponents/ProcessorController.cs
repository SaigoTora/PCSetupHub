using Microsoft.AspNetCore.Mvc;

using PCSetupHub.Data.Models.Attributes;
using PCSetupHub.Data.Models.Hardware;
using PCSetupHub.Data.Repositories.Base;
using PCSetupHub.Data.Repositories.Interfaces.PcConfigurations;
using PCSetupHub.Data.Repositories.Interfaces.Users;

namespace PCSetupHub.Web.Controllers.HardwareComponents
{
	[Route("[Controller]")]
	public class ProcessorController : HardwareBaseController<Processor>
	{
		protected override string ComponentName => "Processor";
		protected override bool IsComponentColorful => false;
		protected override PcConfigurationIncludes PcConfigurationIncludes =>
			PcConfigurationIncludes.Processor;

		public ProcessorController(IPcConfigurationRepository pcConfigRepository,
			IRepository<Processor> processorRepository, IRepository<Color> colorRepository,
			IUserRepository userRepository)
			: base(pcConfigRepository, processorRepository, colorRepository, userRepository)
		{ }

		protected override Processor? GetComponent(PcConfiguration pcConfiguration)
			=> pcConfiguration.Processor;
		protected override void ChangeComponent(PcConfiguration pcConfiguration,
			Processor component)
		{
			pcConfiguration.ChangeProcessor(component);
		}
		protected override void ClearComponent(PcConfiguration pcConfiguration)
			=> pcConfiguration.ClearProcessor();
		protected override bool IsComponentInPcConfig(PcConfiguration pcConfig, int componentId)
			=> pcConfig.Processor?.Id == componentId;
	}
}