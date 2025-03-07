using Microsoft.AspNetCore.Mvc;

using PcSetupHub.Core.Interfaces;
using PCSetupHub.Core.DTOs;

namespace PCSetupHub.Controllers
{
	public class AuthController(IUserService userService) : Controller
	{
		private readonly IUserService _userService = userService;

		[HttpPost]
		public async Task<IActionResult> Register([FromBody] RegisterRequest request)
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