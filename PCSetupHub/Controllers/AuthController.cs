using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using System.Security.Authentication;

using PCSetupHub.Core.DTOs;
using PCSetupHub.Core.Exceptions;
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
		public async Task<IActionResult> Register(RegisterRequest registerRequest)
		{
			TempData["Password"] = registerRequest.Password ?? string.Empty;
			TempData["ConfirmPassword"] = registerRequest.ConfirmPassword ?? string.Empty;

			if (!ModelState.IsValid)
			{
				SetFirstError();
				return View();
			}

			try
			{
				await _userService.RegisterAsync(registerRequest.Login, registerRequest.Password,
					registerRequest.Name, registerRequest.Email);

				AuthResponse authResponse = await _userService.LoginAsync(registerRequest.Login,
					registerRequest.Password);

				AddTokensToCookies(authResponse);
			}
			catch (UserAlreadyExistsException ex)
			{
				ModelState.AddModelError("Login", ex.Message);
				SetFirstError();
				return View();
			}
			catch (EmailAlreadyExistsException ex)
			{
				ModelState.AddModelError("Email", ex.Message);
				SetFirstError();
				return View();
			}

			TempData.Remove("Password");
			TempData.Remove("ConfirmPassword");
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
			if (!ModelState.IsValid)
			{
				SetFirstError();
				return View();
			}

			try
			{
				AuthResponse authResponse = await _userService.LoginAsync(loginRequest.Login,
				loginRequest.Password);

				AddTokensToCookies(authResponse);
			}
			catch (AuthenticationException)
			{
				ModelState.AddModelError("Login", "Invalid login or password.");
				SetFirstError();
				return View();
			}

			return RedirectToAction("Index", "Home");
		}

		private void SetFirstError()
		{
			ViewData["FirstError"] = ModelState.Values
				.SelectMany(v => v.Errors)
				.Select(e => e.ErrorMessage)
				.FirstOrDefault();
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