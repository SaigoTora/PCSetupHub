using Microsoft.AspNetCore.Mvc;

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
		protected override PcConfigurationIncludes PcConfigurationIncludes =>
			PcConfigurationIncludes.Processor;

		public ProcessorController(IPcConfigurationRepository pcConfigRepository,
			IRepository<Processor> processorRepository, IUserRepository userRepository)
			: base(pcConfigRepository, processorRepository, userRepository)
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

		public IActionResult Edit()
		{
			return Ok($"Edit from {GetType().Name}");
		}

		public IActionResult Delete()
		{
			return Ok($"Delete from {GetType().Name}");
		}
	}
}