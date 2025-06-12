using Microsoft.AspNetCore.Mvc;

using PCSetupHub.Core.Extensions;
using PCSetupHub.Data.Models.Hardware;
using PCSetupHub.Data.Repositories.Base;
using PCSetupHub.Data.Repositories.Interfaces.PcConfigurations;
using PCSetupHub.Data.Repositories.Interfaces.Users;
using PCSetupHub.Web.ViewModels;

namespace PCSetupHub.Web.Controllers.HardwareComponents
{
	public class ProcessorController(IPcConfigurationRepository _pcConfigRepository,
		IRepository<Processor> _processorRepository, IUserRepository _userRepository)
		: Controller
	{
		[HttpGet("Processor/Search/{pcConfigurationId}")]
		public async Task<IActionResult> SearchAsync(int pcConfigurationId,
			SearchComponentViewModel<Processor> model)
		{
			int? userId = User.GetId();
			if (!userId.HasValue || !await _userRepository.UserHasPcConfigurationAsync(userId.Value,
				pcConfigurationId))
				return StatusCode(403);

			PcConfiguration? pcConfig = await _pcConfigRepository.GetByIdAsync(pcConfigurationId,
				true);
			if (pcConfig == null)
				return NotFound();

			model.CurrentComponentId = pcConfig.ProcessorId;

			var processors = await _processorRepository
				.GetSomeAsync(p => p.IsDefault);

			processors = [.. processors
				.Where(p => string.IsNullOrEmpty(model.SearchQuery)
					|| p.DisplayName.Contains(model.SearchQuery,
						StringComparison.CurrentCultureIgnoreCase))];

			model.Components = processors;

			return View("Search", model);
		}
		public IActionResult Clear()
		{
			return Ok($"Clear from {GetType().Name}");
		}
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