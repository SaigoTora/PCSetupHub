using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

using PCSetupHub.Core.DTOs;
using PCSetupHub.Core.Interfaces;
using PCSetupHub.Core.Settings;

namespace PCSetupHub.Controllers
{
	public class AuthController(IUserService userService, IOptions<AuthSettings> options) : Controller
	{
		private readonly IUserService _userService = userService;
		private readonly IOptions<AuthSettings> _options = options;

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
			AuthResponse authResponse = await _userService.LoginAsync(loginRequest.Login,
			loginRequest.Password);

			TokenSettings accessTokenSettings = _options.Value.AccessToken;
			TokenSettings refreshTokenSettings = _options.Value.RefreshToken;

			HttpContext.Response.Cookies.Append(accessTokenSettings.CookieName,
				authResponse.AccessToken!,
				new CookieOptions { Expires = DateTime.UtcNow.Add(refreshTokenSettings.Lifetime) });
			HttpContext.Response.Cookies.Append(refreshTokenSettings.CookieName,
				authResponse.RefreshToken!,
				new CookieOptions { Expires = DateTime.UtcNow.Add(refreshTokenSettings.Lifetime) });

			return Ok();
		}
	}
}