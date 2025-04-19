using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.Extensions.Options;
using System.Security.Authentication;
using System.Security.Claims;

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
		private readonly TokenSettings _accessTokenSettings = options.Value.AccessToken;
		private readonly TokenSettings _refreshTokenSettings = options.Value.RefreshToken;

		public async Task<IActionResult> Register()
		{
			if (await IsUserLoggedIn())
				return RedirectToAction("Index", "Home");

			return View();
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		[EnableRateLimiting("LimitPerUser")]
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
				await _userService.RegisterAsync(registerRequest.Login, registerRequest.Password!,
					registerRequest.Name, registerRequest.Email);

				AuthResponse authResponse = await _userService.LoginAsync(registerRequest.Login,
					registerRequest.Password!);

				AddTokensToCookies(authResponse, false);
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

		[HttpGet]
		public async Task<IActionResult> Login(LoginRequest loginRequest)
		{
			if (await IsUserLoggedIn())
				return RedirectToAction("Index", "Home");

			return View(loginRequest);
		}

		[HttpPost, ActionName("Login")]
		[ValidateAntiForgeryToken]
		[EnableRateLimiting("LimitPerUser")]
		public async Task<IActionResult> LoginSubmit(LoginRequest loginRequest)
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

				AddTokensToCookies(authResponse, !loginRequest.RememberMe);
			}
			catch (AuthenticationException)
			{
				ModelState.AddModelError("Login", "Invalid login or password.");
				SetFirstError();
				return View();
			}

			return RedirectToAction("Index", "Home");
		}

		[HttpGet]
		[EnableRateLimiting("LimitPerUser")]
		public IActionResult GoogleLogin()
		{
			var redirectUrl = Url.Action("GoogleResponse", "Auth");
			var properties = new AuthenticationProperties { RedirectUri = redirectUrl };
			return Challenge(properties, GoogleDefaults.AuthenticationScheme);
		}
		[HttpGet]
		public async Task<IActionResult> GoogleResponse()
		{
			const string DEFAULT_NAME = "User";
			var result = await HttpContext.AuthenticateAsync(CookieAuthenticationDefaults.AuthenticationScheme);

			if (result == null || !result.Succeeded)
				return HandleGoogleAuthError();

			await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
			var claims = result.Principal.Identities.FirstOrDefault()?.Claims;

			string? googleId = claims?.FirstOrDefault(
				c => c.Type == ClaimTypes.NameIdentifier)?.Value;
			string? email = claims?.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value;
			string? name = claims?.FirstOrDefault(c => c.Type == ClaimTypes.GivenName)?.Value;

			if (googleId == null || email == null)
				return HandleGoogleAuthError();

			if (string.IsNullOrWhiteSpace(name))
				name = DEFAULT_NAME;

			AuthResponse authResponse = await _userService.LoginOrRegisterByGoogleId(googleId!,
				email!, name!);
			AddTokensToCookies(authResponse, false);

			return RedirectToAction("Index", "Home");
		}

		[HttpGet]
		public async Task<IActionResult> Logout()
		{
			DeleteTokensCookies();
			await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);

			return RedirectToAction("Index", "Home");
		}


		private void SetFirstError()
		{
			ViewData["FirstError"] = ModelState.Values
				.SelectMany(v => v.Errors)
				.Select(e => e.ErrorMessage)
				.FirstOrDefault();
		}
		private ViewResult HandleGoogleAuthError()
		{
			ModelState.AddModelError("Login", "Google authentication failed.");
			SetFirstError();
			return View("Login");
		}
		private void AddTokensToCookies(AuthResponse authResponse, bool isSessionCookies)
		{
			DateTime? accessTokenExpires = isSessionCookies ?
				null : DateTime.UtcNow.Add(_refreshTokenSettings.Lifetime);
			DateTime? refreshTokenExpires = isSessionCookies ?
				null : DateTime.UtcNow.Add(_refreshTokenSettings.Lifetime);

			HttpContext.Response.Cookies.Append(_accessTokenSettings.CookieName,
				authResponse.AccessToken!,
				new CookieOptions { Expires = accessTokenExpires });

			HttpContext.Response.Cookies.Append(_refreshTokenSettings.CookieName,
				authResponse.RefreshToken!,
				new CookieOptions { Expires = refreshTokenExpires });
		}
		private void DeleteTokensCookies()
		{
			HttpContext.Response.Cookies.Delete(_accessTokenSettings.CookieName);
			HttpContext.Response.Cookies.Delete(_refreshTokenSettings.CookieName);
		}
		private async Task<bool> IsUserLoggedIn()
		{
			string? accessToken = HttpContext.Request.Cookies[_accessTokenSettings.CookieName];
			string? refreshToken = HttpContext.Request.Cookies[_refreshTokenSettings.CookieName];

			return await _userService.IsUserLoggedIn(accessToken!, refreshToken!);
		}
	}
}