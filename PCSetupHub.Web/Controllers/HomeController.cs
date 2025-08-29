using Microsoft.AspNetCore.Mvc;

using PCSetupHub.Core.External.NewsApi;
using PCSetupHub.Core.Interfaces;

namespace PCSetupHub.Web.Controllers
{
	public class HomeController : Controller
	{
		private readonly ILogger<HomeController> _logger;
		private readonly INewsApiService _newsApiService;
		private readonly ICacheService _cacheService;

		public HomeController(ILogger<HomeController> logger, INewsApiService newsApiService,
			ICacheService cacheService)
		{
			_logger = logger;
			_newsApiService = newsApiService;
			_cacheService = cacheService;
		}

		[HttpGet]
		public async Task<IActionResult> Index()
		{
			const string CACHE_KEY = "News:Technology";
			TimeSpan expiration = TimeSpan.FromHours(1);
			Article[]? articles = await _cacheService.GetAsync<Article[]>(CACHE_KEY);

			if (articles == null)
			{
				articles = await _newsApiService.GetTechnologyHeadlinesAsync();
				if (articles != null && articles.Length > 0)
					await _cacheService.SetAsync(CACHE_KEY, articles, expiration);
			}

			return View(articles);
		}

		[HttpGet]
		public IActionResult Privacy() => View();
	}
}