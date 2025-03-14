using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

using PCSetupHub.Core.DTOs;
using PCSetupHub.Core.Interfaces;
using PCSetupHub.Core.Settings;

namespace PCSetupHub.Controllers
{
	public class AuthController(IUserService userService, IOptions<AuthSettings> options) 
		: Controller
	{
		private readonly IUserService _userService = userService;
		private readonly IOptions<AuthSettings> _options = options;

		public async Task<IActionResult> Register()
		{
			if (await IsUserLoggedIn())
				return RedirectToAction("Index", "Home");

			return View();
		}

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

		public async Task<IActionResult> Login()
		{
			if (await IsUserLoggedIn())
				return RedirectToAction("Index", "Home");

			return View();
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> Login(LoginRequest loginRequest)
		{
			AuthResponse authResponse = await _userService.LoginAsync(loginRequest.Login,
			loginRequest.Password);

			AddTokensToCookies(authResponse);

			return RedirectToAction("Index", "Home");
		}

		private void AddTokensToCookies(AuthResponse authResponse)
		{
			TokenSettings accessTokenSettings = _options.Value.AccessToken;
			TokenSettings refreshTokenSettings = _options.Value.RefreshToken;

			HttpContext.Response.Cookies.Append(accessTokenSettings.CookieName,
				authResponse.AccessToken!,
				new CookieOptions { Expires = DateTime.UtcNow.Add(refreshTokenSettings.Lifetime) });

			HttpContext.Response.Cookies.Append(refreshTokenSettings.CookieName,
				authResponse.RefreshToken!,
				new CookieOptions { Expires = DateTime.UtcNow.Add(refreshTokenSettings.Lifetime) });
		}
		private async Task<bool> IsUserLoggedIn()
		{
			TokenSettings accessTokenSettings = _options.Value.AccessToken;
			TokenSettings refreshTokenSettings = _options.Value.RefreshToken;

			string? accessToken = HttpContext.Request.Cookies[accessTokenSettings.CookieName];
			string? refreshToken = HttpContext.Request.Cookies[refreshTokenSettings.CookieName];

			return await _userService.IsUserLoggedIn(accessToken!, refreshToken!);
		}
	}
}