using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using PCSetupHub.Core.DTOs;
using PCSetupHub.Core.Extensions;
using PCSetupHub.Data.Models.Users;
using PCSetupHub.Data.Repositories.Interfaces.Users;

namespace PCSetupHub.Web.Controllers
{
	public class SettingsController : Controller
	{
		private readonly IUserRepository _userRepository;

		public SettingsController(IUserRepository userRepository)
		{
			_userRepository = userRepository;
		}

		[HttpGet("Settings/{login}")]
		[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
		public async Task<IActionResult> Index(string login)
		{
			User? user = await _userRepository.GetByLoginAsync(login,
				UserIncludes.PcConfiguration | UserIncludes.PrivacySetting);
			if (user == null)
				return NotFound();
			if (user.Id != User.GetId())
				return StatusCode(403);

			UserSettings userSettings = new(user);

			return View(userSettings);
		}

		[HttpPost("Settings/{login}")]
		[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
		public async Task<IActionResult> Index(string login, UserSettings model)
		{
			User? user = await _userRepository.GetByLoginAsync(login,
				UserIncludes.PcConfiguration | UserIncludes.PrivacySetting);
			if (user == null)
				return NotFound();
			if (user.Id != User.GetId())
				return StatusCode(403);
			model.SetMeta(user.Login, user.AvatarUrl, user.PcConfiguration.Id, user.HasPassword);

			if (await this.HandleInvalidModelStateAsync(model) is ViewResult errorResult)
				return errorResult;

			if (model.Email != user.Email && await _userRepository.ExistsByEmailAsync(model.Email))
			{
				ModelState.AddModelError(nameof(model.Email), $"User with email '{model.Email}' " +
					$"already exists.");
				this.SetFirstError();
				return View(model);
			}

			PrivacySetting privacySettings = new(model.FriendsAccessId, model.FollowersAccessId,
				model.FollowingsAccessId, model.MessagesAccessId, model.PcConfigAccessId,
				model.CommentWritingAccessId);
			user.UpdateSettings(model.Name, model.Email, model.Description, privacySettings);

			await _userRepository.UpdateAsync(user);

			return RedirectToAction("Index", new { login = user.Login });
		}
	}
}