using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using PCSetupHub.Data.Models.Users;
using PCSetupHub.Data.Repositories.Interfaces;

namespace PCSetupHub.Web.Controllers
{
	public class ProfileController(IUserRepository _userRepository) : Controller
	{
		[Route("Profile/{login?}")]
		[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
		public async Task<IActionResult> Index(string login)
		{
			if (string.IsNullOrWhiteSpace(login))
				return BadRequest();

			User? user = await _userRepository.GetByLoginAsync(login, true);
			if (user == null)
				return NotFound();

			return View(user);
		}
	}
}