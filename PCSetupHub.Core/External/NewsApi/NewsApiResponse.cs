namespace PCSetupHub.Core.External.NewsApi
{
	public class NewsApiResponse
	{
		public string Status { get; set; } = string.Empty;
		public int TotalResults { get; set; }
		public Article[] Articles { get; set; } = [];
	}
}