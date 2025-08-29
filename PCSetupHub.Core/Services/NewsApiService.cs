using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Net.Http.Json;

using PCSetupHub.Core.External.NewsApi;
using PCSetupHub.Core.Interfaces;

namespace PCSetupHub.Core.Services
{
	public class NewsApiService : INewsApiService
	{
		private readonly ILogger<NewsApiService> _logger;
		private readonly IConfiguration _configuration;
		private readonly HttpClient _httpClient;

		public NewsApiService(ILogger<NewsApiService> logger, IConfiguration configuration,
			HttpClient httpClient)
		{
			_logger = logger;
			_configuration = configuration;
			_httpClient = httpClient;
		}

		public async Task<Article[]> GetTechnologyHeadlinesAsync()
		{
			string apiKey = (await AzureSecretService.GetSecretAsync(_configuration, "NewsApiKey"))
				.Value;
			string category = "technology";

			string url = $"https://newsapi.org/v2/top-headlines?apiKey={apiKey}" +
				$"&category={category}";

			NewsApiResponse? response = await _httpClient.GetFromJsonAsync<NewsApiResponse>(url);
			if (response == null || response.Status != "ok")
			{
				_logger.LogWarning("News API returned an error or null data: {@Response}",
					response);
				return [];
			}

			return response.Articles;
		}
	}
}