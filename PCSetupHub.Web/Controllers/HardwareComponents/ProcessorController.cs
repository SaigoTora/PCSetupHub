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
		IRepository<Processor> _processorRepository, IUserRepository _userRepository,
		IRepository<PcConfiguration> _genericPcConfigRepository)
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

		[HttpPost("Processor/Select/{pcConfigurationId}/{componentId}")]
		public async Task<IActionResult> SelectAsync(int pcConfigurationId, int componentId)
		{
			int? userId = User.GetId();
			if (!userId.HasValue || !await _userRepository.UserHasPcConfigurationAsync(userId.Value,
				pcConfigurationId))
				return StatusCode(403);

			PcConfiguration? pcConfig = await _pcConfigRepository.GetByIdAsync(pcConfigurationId,
				true);
			if (pcConfig == null)
				return NotFound();

			Processor? processor = await _processorRepository.GetOneAsync(componentId);
			if (processor == null)
				return NotFound();
			if (!processor.IsDefault)
				return StatusCode(403);

			pcConfig.ChangeProcessor(processor);
			await _genericPcConfigRepository.UpdateAsync(pcConfig);

			return RedirectToAction("Index", "PcSetup", new { id = pcConfigurationId });
		}
		[HttpPost("Processor/Clear/{pcConfigurationId}")]
		public async Task<IActionResult> ClearAsync(int pcConfigurationId)
		{
			int? userId = User.GetId();
			if (!userId.HasValue || !await _userRepository.UserHasPcConfigurationAsync(userId.Value,
				pcConfigurationId))
				return StatusCode(403);

			PcConfiguration? pcConfig = await _pcConfigRepository.GetByIdAsync(pcConfigurationId,
				true);
			if (pcConfig == null)
				return NotFound();

			if (pcConfig.Processor != null && !pcConfig.Processor.IsDefault)
				return StatusCode(403);

			pcConfig.ClearProcessor();
			await _genericPcConfigRepository.UpdateAsync(pcConfig);

			return RedirectToAction("Index", "PcSetup", new { id = pcConfigurationId });
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