using Microsoft.AspNetCore.Mvc;

using PCSetupHub.Core.External.NewsApi;
using PCSetupHub.Core.Interfaces;

namespace PCSetupHub.Web.Controllers
{
	public class HomeController : Controller
	{
		private readonly ILogger<HomeController> _logger;
		private readonly INewsApiService _newsApiService;

		public HomeController(ILogger<HomeController> logger, INewsApiService newsApiService)
		{
			_logger = logger;
			_newsApiService = newsApiService;
		}
		[HttpGet]
		public async Task<IActionResult> Index()
		{
			Article[] articles = await _newsApiService.GetTechnologyHeadlinesAsync();

			return View(articles);
		}

		[HttpGet]
		public IActionResult Privacy() => View();
	}
}