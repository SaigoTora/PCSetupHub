using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace PCSetupHub.Controllers
{
	public class ProfileController : Controller
	{
		[HttpGet]
		[Route("profile/{login?}")]
		[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
		public IActionResult Index(string login, int id)
		{
			return Ok($"login: {login};\nid: {id}");
		}
	}
}