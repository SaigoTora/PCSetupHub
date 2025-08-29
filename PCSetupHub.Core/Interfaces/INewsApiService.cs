using PCSetupHub.Core.External.NewsApi;

namespace PCSetupHub.Core.Interfaces
{
	public interface INewsApiService
	{
		/// <summary>
		/// Retrieves the latest technology news articles from the News API.
		/// Returns an empty array if the API returns an error or no articles are available.
		/// </summary>
		/// <returns>
		/// An array of <see cref="Article"/> objects representing the news articles.
		/// </returns>
		Task<Article[]> GetTechnologyHeadlinesAsync();
	}
}