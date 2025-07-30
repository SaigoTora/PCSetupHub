using Microsoft.AspNetCore.Mvc;

namespace PCSetupHub.Web.Controllers
{
	public class ErrorController : Controller
	{
		[HttpGet]
		[Route("Error/{statusCode}")]
		[ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
		public IActionResult Index(int statusCode)
		{
			return statusCode switch
			{
				400 => View("BadRequest"),
				401 => View("Unauthorized"),
				403 => View("Forbidden"),
				404 => View("NotFound"),
				409 => View("Conflict"),
				429 => View("TooManyRequests"),
				_ => View("GeneralError"),
			};
		}
	}
}