using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using PCSetupHub.Core.Extensions;
using PCSetupHub.Data.Models.Hardware;
using PCSetupHub.Data.Repositories.Base;
using PCSetupHub.Data.Repositories.Interfaces.PcConfigurations;

namespace PCSetupHub.Web.Controllers
{
	public class PcSetupController(ILogger<PcSetupController> _logger,
		IPcConfigurationRepository _pcConfigRepository, IRepository<PcType> _pcTypeRepository)
		: Controller
	{
		[HttpGet("PcSetup/{id}")]
		public async Task<IActionResult> Index(int id)
		{
			PcConfiguration? pcConfig = await _pcConfigRepository.GetByIdAsync(id, true);

			if (pcConfig == null)
				return NotFound();

			return View(pcConfig);
		}

		[HttpPost("UpdateType/{id}")]
		[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
		public async Task<IActionResult> UpdateType(int id, string typeName)
		{
			PcConfiguration? pcConfiguration
				= await _pcConfigRepository.GetByIdAsync(id, false);

			if (pcConfiguration == null)
				return NotFound();

			int userId = User.GetId() ?? -1;
			if (pcConfiguration.User != null && pcConfiguration.User.Id != userId)
				return StatusCode(403);

			// There should be no more than 1 element since the Name field is unique
			var pcType = await _pcTypeRepository.GetSomeAsync(x => x.Name == typeName);
			if (pcType == null || pcType.Count == 0 || pcType[0] == null)
				return NotFound();

			try
			{
				pcConfiguration.ChangeType(pcType[0]);
				await _pcConfigRepository.UpdateAsync(pcConfiguration);
				_logger.LogInformation("User {UserId} updated PcConfiguration {Id} " +
					"to type '{TypeName}'", userId, id, typeName);
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "User {UserId} failed to update PcConfiguration {Id} " +
					"to type '{TypeName}'", userId, id, typeName);
				throw;
			}

			return RedirectToAction("Index", new { id });
		}
	}
}