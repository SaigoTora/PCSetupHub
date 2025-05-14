using Microsoft.AspNetCore.Mvc;

using PCSetupHub.Data.Models.Hardware;
using PCSetupHub.Data.Repositories.Interfaces.PcConfigurations;

namespace PCSetupHub.Web.Controllers
{
	public class PcSetupController(IPcConfigurationRepository _pcConfigRepository)
		: Controller
	{
		[HttpGet("PcSetup/{id}")]
		public async Task<IActionResult> Index(int id)
		{
			PcConfiguration? pcConfig = await _pcConfigRepository.GetByIdAsync(id);

			if (pcConfig == null)
				return NotFound();

			return View(pcConfig);
		}
	}
}