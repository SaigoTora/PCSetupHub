using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.Extensions.Options;
using System.Security.Authentication;
using System.Security.Claims;

using PCSetupHub.Core.DTOs;
using PCSetupHub.Core.Exceptions;
using PCSetupHub.Core.Extensions;
using PCSetupHub.Core.Interfaces;
using PCSetupHub.Core.Settings;
using PCSetupHub.Data.Models.Users;
using PCSetupHub.Data.Repositories.Interfaces.Users;

namespace PCSetupHub.Web.Controllers
{
	public class AuthController : Controller
	{
		private readonly ILogger<AuthController> _logger;
		private readonly IUserService _userService;
		private readonly IUserRepository _userRepository;
		private readonly TokenSettings _accessTokenSettings;
		private readonly TokenSettings _refreshTokenSettings;

		public AuthController(ILogger<AuthController> logger, IUserService userService,
			IUserRepository userRepository, IOptions<AuthSettings> options)
		{
			_logger = logger;
			_userService = userService;
			_userRepository = userRepository;
			_accessTokenSettings = options.Value.AccessToken;
			_refreshTokenSettings = options.Value.RefreshToken;
		}

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
				this.SetFirstError();
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

			ClearRegisterPasswordsFromTempData();
			return RedirectToAction("Index", "Home");
		}

		private void SavePasswordsToTempData(RegisterRequest request)
		{
			TempData[nameof(RegisterRequest.Password)] = request.Password ?? string.Empty;
			TempData[nameof(RegisterRequest.ConfirmPassword)] = request.ConfirmPassword ?? string.Empty;
		}
		private void ClearRegisterPasswordsFromTempData()
		{
			TempData.Remove(nameof(RegisterRequest.Password));
			TempData.Remove(nameof(RegisterRequest.ConfirmPassword));
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

			this.SetFirstError();
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
				this.SetFirstError();
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
				this.SetFirstError();
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
				googleId ??= "Unknown";
				email ??= "Unknown";
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
			this.SetFirstError();

			return View("Login");
		}
		#endregion

		#region Updating
		[HttpGet("UpdateLogin/{login}")]
		[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
		public async Task<IActionResult> UpdateLogin(string login)
		{
			var (authResult, user) = await CheckUserAuthorizationAsync(login);
			if (authResult != null)
				return authResult;

			UpdateLoginRequest loginRequest = new(user.Login);
			return View(loginRequest);
		}

		[HttpPost("UpdateLogin/{login}")]
		[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
		public async Task<IActionResult> UpdateLogin(string login, UpdateLoginRequest model)
		{
			var (authResult, user) = await CheckUserAuthorizationAsync(login);
			if (authResult != null)
				return authResult;

			if (!ModelState.IsValid)
			{
				this.SetFirstError();
				return View(model);
			}

			if (await ValidateLoginChangeAsync(model, user) is IActionResult validationResult)
				return validationResult;

			user.SetLogin(model.NewLogin);
			await _userRepository.UpdateAsync(user);

			bool rememberMe = User.GetRememberMe();
			AuthResponse authResponse = await _userService.LoginAsync(model.NewLogin,
				model.Password, rememberMe);

			DeleteTokensCookies();
			AddTokensToCookies(authResponse, !rememberMe);
			_logger.LogInformation("User with Id {UserId} changed login from '{OldLogin}' to " +
				"'{NewLogin}'", user.Id, login, model.NewLogin);

			return RedirectToSettings(user.Login);
		}

		[HttpGet("UpdatePassword/{login}")]
		[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
		public async Task<IActionResult> UpdatePassword(string login)
		{
			var (authResult, _) = await CheckUserAuthorizationAsync(login);
			if (authResult != null)
				return authResult;

			return View(new UpdatePasswordRequest());
		}

		[HttpPost("UpdatePassword/{login}")]
		[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
		public async Task<IActionResult> UpdatePassword(string login, UpdatePasswordRequest model)
		{
			SavePasswordsToTempData(model);
			var (authResult, user) = await CheckUserAuthorizationAsync(login);
			if (authResult != null)
				return authResult;

			if (!ModelState.IsValid)
			{
				this.SetFirstError();
				return View(model);
			}

			if (ValidatePasswordChange(model, user) is IActionResult validationResult)
				return validationResult;

			user.SetPasswordHash(_userService.HashPassword(user, model.NewPassword));
			await _userRepository.UpdateAsync(user);
			_logger.LogInformation("User with Id {UserId} successfully changed password", user.Id);
			ClearUpdatePasswordsFromTempData();

			return RedirectToSettings(user.Login);
		}

		private async Task<(IActionResult?, User)> CheckUserAuthorizationAsync(string login)
		{
			var user = await _userRepository.GetByLoginAsync(login, false);
			if (user == null)
				return (NotFound(), null!);
			if (user.Id != User.GetId())
				return (StatusCode(403), user);

			return (null, user);
		}
		private async Task<ViewResult?> ValidateLoginChangeAsync(UpdateLoginRequest request,
			User user)
		{
			string oldLogin = user.Login;

			if (oldLogin == request.NewLogin)
			{
				ModelState.AddModelError(nameof(request.NewLogin),
					"New login cannot be the same as the current login.");
				this.SetFirstError();
				return View(request);
			}

			if (!_userService.VerifyPassword(user, request.Password))
			{
				ModelState.AddModelError(nameof(request.Password), "Invalid password.");
				this.SetFirstError();
				return View(request);
			}

			if (request.NewLogin != oldLogin
				&& await _userRepository.ExistsByLoginAsync(request.NewLogin))
			{
				ModelState.AddModelError(nameof(request.NewLogin),
					$"User with login '{request.NewLogin}' already exists.");
				this.SetFirstError();
				return View(request);
			}

			return null;
		}
		private ViewResult? ValidatePasswordChange(UpdatePasswordRequest request,
			User user)
		{
			if (request.OldPassword == request.NewPassword)
			{
				ModelState.AddModelError(nameof(request.NewPassword),
					"New password cannot be the same as the current password.");
				this.SetFirstError();
				return View(request);
			}

			if (!_userService.VerifyPassword(user, request.OldPassword))
			{
				ModelState.AddModelError(nameof(request.OldPassword), "Invalid old password.");
				this.SetFirstError();
				return View(request);
			}

			return null;
		}

		private void SavePasswordsToTempData(UpdatePasswordRequest request)
		{
			TempData[nameof(UpdatePasswordRequest.OldPassword)] = request.OldPassword ?? string.Empty;
			TempData[nameof(UpdatePasswordRequest.NewPassword)] = request.NewPassword ?? string.Empty;
			TempData[nameof(UpdatePasswordRequest.ConfirmPassword)] = request.ConfirmPassword ?? string.Empty;
		}
		private void ClearUpdatePasswordsFromTempData()
		{
			TempData.Remove(nameof(UpdatePasswordRequest.OldPassword));
			TempData.Remove(nameof(UpdatePasswordRequest.NewPassword));
			TempData.Remove(nameof(UpdatePasswordRequest.ConfirmPassword));
		}
		private RedirectToActionResult RedirectToSettings(string login)
			=> RedirectToAction("Index", "Settings", new { login });
		#endregion

		[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
		public async Task<IActionResult> Logout()
		{
			string login = User.GetLogin() ?? "Unknown";
			await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
			DeleteTokensCookies();
			_logger.LogInformation("User logged out successfully: {Login}", login);

			return RedirectToAction("Index", "Home");
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