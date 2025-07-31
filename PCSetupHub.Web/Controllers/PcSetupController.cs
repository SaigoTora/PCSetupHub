using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;

using PCSetupHub.Core.Extensions;
using PCSetupHub.Core.Interfaces;
using PCSetupHub.Data.Models.Hardware;
using PCSetupHub.Data.Models.Users;
using PCSetupHub.Data.Repositories.Base;
using PCSetupHub.Data.Repositories.Interfaces.PcConfigurations;
using PCSetupHub.Data.Repositories.Interfaces.Users;

namespace PCSetupHub.Web.Controllers
{
	public class PcSetupController : Controller
	{
		private readonly ILogger<PcSetupController> _logger;
		private readonly IPcConfigurationRepository _pcConfigRepository;
		private readonly IRepository<PcType> _pcTypeRepository;
		private readonly IUserRepository _userRepository;
		private readonly IUserAccessService _userAccessService;

		public PcSetupController(ILogger<PcSetupController> logger,
			IPcConfigurationRepository pcConfigRepository, IRepository<PcType> pcTypeRepository,
			IUserRepository userRepository, IUserAccessService userAccessService)
		{
			_logger = logger;
			_pcConfigRepository = pcConfigRepository;
			_pcTypeRepository = pcTypeRepository;
			_userRepository = userRepository;
			_userAccessService = userAccessService;
		}

		[HttpGet("PcSetup/{id}")]
		public async Task<IActionResult> Index(int id)
		{
			PcConfiguration? pcConfig = await _pcConfigRepository.GetByIdAsync(id, true);
			if (pcConfig == null || pcConfig.User == null)
				return NotFound();

			User? user = await _userRepository.GetByLoginAsync(pcConfig.User.Login,
				UserIncludes.PrivacySetting);
			if (user == null)
				return NotFound();

			string currentUserLogin = User.GetLogin() ?? string.Empty;
			bool pcConfigAccessGranted = await _userAccessService.HasAccessAsync(currentUserLogin,
				user.Login, (PrivacyLevelType)user.PrivacySetting.PcConfigAccessId);
			if (!pcConfigAccessGranted)
				return StatusCode(403);

			return View(pcConfig);
		}

		[HttpPost("UpdateType/{id}")]
		[EnableRateLimiting("PcConfiguration")]
		[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
		public async Task<IActionResult> UpdateType(int id, string typeName)
		{
			PcConfiguration? pcConfiguration
				= await _pcConfigRepository.GetByIdAsync(id, false, false);

			if (pcConfiguration == null)
				return NotFound();

			int userId = User.GetId() ?? -1;
			if (pcConfiguration.User != null && pcConfiguration.UserId != userId)
				return StatusCode(403);

			// There should be no more than 1 element since the Name field is unique
			var pcType = await _pcTypeRepository.GetSomeAsync(x => x.Name == typeName);
			if (pcType == null || pcType.Count == 0 || pcType[0] == null)
				return NotFound();

			try
			{
				pcConfiguration.Type = pcType[0];
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