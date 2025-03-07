using Microsoft.AspNetCore.Mvc;

using PCSetupHub.Core.Interfaces;
using PCSetupHub.Core.DTOs;

namespace PCSetupHub.Controllers
{
	public class AuthController(IUserService userService) : Controller
	{
		private readonly IUserService _userService = userService;

		public IActionResult Register() => View();

		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> Register(RegisterRequest request)
		{
			if (!ModelState.IsValid)
				return BadRequest(ModelState);

			await _userService.RegisterAsync(request.Login, request.Password, request.Name,
				request.Email);

			return RedirectToAction("Index", "Home");
		}

		public IActionResult Login() => View();

		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> Login(LoginRequest loginRequest)
		{
			string token = await _userService.LoginAsync(loginRequest.Login,
				loginRequest.Password);

			return Ok(token);
		}
	}
}