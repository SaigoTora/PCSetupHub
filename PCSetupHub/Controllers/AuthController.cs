using Microsoft.AspNetCore.Mvc;

using PCSetupHub.DTOs;
using PCSetupHub.Services;

namespace PCSetupHub.Controllers
{
	public class AuthController(UserService userService) : Controller
	{
		private readonly UserService _userService = userService;

		[HttpPost]
		public async Task<IActionResult> Register([FromBody] RegisterUserRequest request)
		{
			await _userService.RegisterAsync(request.Login, request.Password, request.Name,
				request.Email);
			return NoContent();
		}
		[HttpPost]
		public async Task<IActionResult> Login([FromBody] LoginRequest loginRequest)
		{
			string token = await _userService.LoginAsync(loginRequest.Login,
				loginRequest.Password);
			return Ok(token);
		}
	}
}