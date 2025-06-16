using Microsoft.AspNetCore.Mvc;

using PCSetupHub.Data.Models.Hardware;
using PCSetupHub.Data.Repositories.Base;
using PCSetupHub.Data.Repositories.Interfaces.PcConfigurations;
using PCSetupHub.Data.Repositories.Interfaces.Users;
using PCSetupHub.Web.ViewModels;

namespace PCSetupHub.Web.Controllers.HardwareComponents
{
	public class ProcessorController : HardwareBaseController<Processor>
	{
		private readonly IPcConfigurationRepository _pcConfigRepository;
		private readonly IRepository<Processor> _processorRepository;

		public ProcessorController(IPcConfigurationRepository pcConfigRepository,
			IRepository<Processor> processorRepository, IUserRepository userRepository)
			: base(processorRepository, userRepository)
		{
			_pcConfigRepository = pcConfigRepository;
			_processorRepository = processorRepository;
		}

		[HttpGet("Processor/Search/{pcConfigurationId}")]
		public async Task<IActionResult> SearchAsync(int pcConfigurationId, int page = 1,
			string? searchQuery = null)
		{
			if (!await HasAccessToPcConfigurationAsync(pcConfigurationId))
				return StatusCode(403);

			PcConfiguration? pcConfig = await _pcConfigRepository.GetByIdAsync(pcConfigurationId,
				true);
			if (pcConfig == null)
				return NotFound();

			var filteredComponents = await GetFilteredComponentsAsync(searchQuery);
			int totalItems = filteredComponents.Count;

			SearchComponentViewModel model = new(pcConfigurationId, searchQuery,
				pcConfig.Processor?.Id, "Processor", page, totalItems);

			model.Components = GetPagedItems(filteredComponents, model.Page, model.PageSize);

			return View("Search", model);
		}

		[HttpPost("Processor/Select/{pcConfigurationId}/{componentId}")]
		public async Task<IActionResult> SelectAsync(int pcConfigurationId, int componentId)
		{
			if (!await HasAccessToPcConfigurationAsync(pcConfigurationId))
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
			await _pcConfigRepository.UpdateAsync(pcConfig);

			return RedirectToPcSetup(pcConfigurationId);
		}

		[HttpPost("Processor/Clear/{pcConfigurationId}")]
		public async Task<IActionResult> ClearAsync(int pcConfigurationId)
		{
			if (!await HasAccessToPcConfigurationAsync(pcConfigurationId))
				return StatusCode(403);

			PcConfiguration? pcConfig = await _pcConfigRepository.GetByIdAsync(pcConfigurationId,
				true);
			if (pcConfig == null)
				return NotFound();

			if (pcConfig.Processor != null && !pcConfig.Processor.IsDefault)
				return StatusCode(403);

			pcConfig.ClearProcessor();
			await _pcConfigRepository.UpdateAsync(pcConfig);

			return RedirectToPcSetup(pcConfigurationId);
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