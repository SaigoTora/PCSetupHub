using Microsoft.AspNetCore.Mvc;

namespace PCSetupHub.Controllers
{
	public class HomeController(ILogger<HomeController> logger) : Controller
	{
		private readonly ILogger<HomeController> _logger = logger;

		public IActionResult Index() => View();
		public IActionResult Privacy() => View();
	}
}