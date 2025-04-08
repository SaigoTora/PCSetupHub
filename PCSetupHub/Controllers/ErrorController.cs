using Microsoft.AspNetCore.Mvc;

namespace PCSetupHub.Controllers
{
	public class ErrorController : Controller
	{
		[Route("Error/{statusCode}")]
		public IActionResult Index(int statusCode)
		{
			return statusCode switch
			{
				404 => View("NotFound"),
				429 => View("TooManyRequests"),
				_ => View("GeneralError"),
			};
		}
	}
}