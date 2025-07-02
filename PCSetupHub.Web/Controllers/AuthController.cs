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
using PCSetupHub.Core.Extensions;

namespace PCSetupHub.Web.Controllers
{
	public class AuthController(ILogger<AuthController> logger, IUserService userService,
		IOptions<AuthSettings> options)
		: Controller
	{
		private readonly ILogger<AuthController> _logger = logger;
		private readonly IUserService _userService = userService;
		private readonly TokenSettings _accessTokenSettings = options.Value.AccessToken;
		private readonly TokenSettings _refreshTokenSettings = options.Value.RefreshToken;

		#region Registration
		public async Task<IActionResult> Register()
		{
			if (await IsUserLoggedInAsync())
				return RedirectToAction("Index", "Home");

			return View();
		}
		[HttpPost]
		[EnableRateLimiting("LimitPerUser")]
		public async Task<IActionResult> Register(RegisterRequest registerRequest)
		{
			SavePasswordsToTempData(registerRequest);

			if (!ModelState.IsValid)
			{
				SetFirstError();
				return View();
			}

			try
			{
				await _userService.RegisterAsync(registerRequest.Login, registerRequest.Password!,
					registerRequest.Name, registerRequest.Email, null);

				AuthResponse authResponse = await _userService.LoginAsync(registerRequest.Login,
					registerRequest.Password!, true);

				AddTokensToCookies(authResponse, false);
				_logger.LogInformation("User registered successfully: {Login}",
					registerRequest.Login);
			}
			catch (Exception ex) when (HandleRegistrationExceptions(ex))
			{
				_logger.LogWarning("Registration failed for user {Login}: {ExceptionMessage}",
					registerRequest.Login, ex.Message);
				return View();
			}

			ClearPasswordsFromTempData();
			return RedirectToAction("Index", "Home");
		}

		private void SavePasswordsToTempData(RegisterRequest request)
		{
			TempData["Password"] = request.Password ?? string.Empty;
			TempData["ConfirmPassword"] = request.ConfirmPassword ?? string.Empty;
		}
		private void ClearPasswordsFromTempData()
		{
			TempData.Remove("Password");
			TempData.Remove("ConfirmPassword");
		}
		private bool HandleRegistrationExceptions(Exception ex)
		{
			switch (ex)
			{
				case UserAlreadyExistsException userEx:
					ModelState.AddModelError("Login", userEx.Message);
					break;
				case EmailAlreadyExistsException emailEx:
					ModelState.AddModelError("Email", emailEx.Message);
					break;
				default:
					return false;
			}

			SetFirstError();
			return true;
		}
		#endregion

		#region Login
		public async Task<IActionResult> Login(LoginRequest loginRequest)
		{
			if (await IsUserLoggedInAsync())
				return RedirectToAction("Index", "Home");

			return View(loginRequest);
		}

		[HttpPost, ActionName("Login")]
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
					loginRequest.Password, loginRequest.RememberMe);

				AddTokensToCookies(authResponse, !loginRequest.RememberMe);
				_logger.LogInformation("User logged in successfully: {Login}", loginRequest.Login);
			}
			catch (AuthenticationException)
			{
				ModelState.AddModelError("Login", "Invalid login or password.");
				SetFirstError();
				_logger.LogWarning("Invalid login attempt for user {Login}", loginRequest.Login);
				return View();
			}

			return RedirectToAction("Index", "Home");
		}
		#endregion

		#region Google login
		[EnableRateLimiting("LimitPerUser")]
		public IActionResult GoogleLogin()
		{
			var redirectUrl = Url.Action("GoogleResponse", "Auth");
			var properties = new AuthenticationProperties { RedirectUri = redirectUrl };

			return Challenge(properties, GoogleDefaults.AuthenticationScheme);
		}
		public async Task<IActionResult> GoogleResponse()
		{
			const string DEFAULT_NAME = "User";
			var result = await HttpContext.AuthenticateAsync(
				CookieAuthenticationDefaults.AuthenticationScheme);

			if (result == null || !result.Succeeded)
				return HandleGoogleAuthError();

			await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
			var claims = result.Principal.Identities.FirstOrDefault()?.Claims;

			string? googleId = claims?.FirstOrDefault(
				c => c.Type == ClaimTypes.NameIdentifier)?.Value;
			string? email = claims?.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value;
			string? name = claims?.FirstOrDefault(c => c.Type == ClaimTypes.GivenName)?.Value;

			if (googleId == null || email == null)
			{
				googleId = googleId ?? "Unknown";
				email = email ?? "Unknown";
				_logger.LogInformation("Google authentication failed for user {Email} " +
					"(GoogleId: {GoogleId})", email, googleId);
				return HandleGoogleAuthError();
			}

			if (string.IsNullOrWhiteSpace(name))
				name = DEFAULT_NAME;

			AuthResponse authResponse = await _userService.LoginOrRegisterByGoogleIdAsync(googleId!,
				email!, name!);
			AddTokensToCookies(authResponse, false);
			_logger.LogInformation("Google authentication succeeded for user {Email} " +
				"(GoogleId: {GoogleId})", email, googleId);

			return RedirectToAction("Index", "Home");
		}

		private ViewResult HandleGoogleAuthError()
		{
			ModelState.AddModelError("Login", "Google authentication failed.");
			SetFirstError();

			return View("Login");
		}
		#endregion

		public async Task<IActionResult> Logout()
		{
			string login = User.GetLogin() ?? "Unknown";
			await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
			DeleteTokensCookies();
			_logger.LogInformation("User logged out successfully: {Login}", login);

			return RedirectToAction("Index", "Home");
		}


		private void SetFirstError()
		{
			ViewData["FirstError"] = ModelState.Values
				.SelectMany(v => v.Errors)
				.Select(e => e.ErrorMessage)
				.FirstOrDefault();
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
		private async Task<bool> IsUserLoggedInAsync()
		{
			string? accessToken = HttpContext.Request.Cookies[_accessTokenSettings.CookieName];
			string? refreshToken = HttpContext.Request.Cookies[_refreshTokenSettings.CookieName];

			return await _userService.IsUserLoggedInAsync(accessToken!, refreshToken!);
		}
	}
}