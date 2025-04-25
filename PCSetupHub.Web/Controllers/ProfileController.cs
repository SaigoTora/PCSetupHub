using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace PCSetupHub.Web.Controllers
{
	public class ProfileController : Controller
	{
		[Route("profile/{login?}")]
		[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
		public IActionResult Index(string login)
		{
			return Ok($"login: {login}");
		}
	}
}