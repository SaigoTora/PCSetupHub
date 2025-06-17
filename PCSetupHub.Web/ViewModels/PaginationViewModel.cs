namespace PCSetupHub.Web.ViewModels
{
	public class PaginationViewModel
	{
		public int Page { get; private set; } = 1;
		public int TotalItems { get; private set; }
		public string ActionName { get; private set; } = string.Empty;
		public int PageSize { get; set; }
		public string? SearchQuery { get; private set; }
		public int TotalPages => (int)Math.Ceiling((double)TotalItems / PageSize);

		public PaginationViewModel()
		{ }
		public PaginationViewModel(int page, int totalItems, string actionName, int pageSize,
			string? searchQuery = null)
		{
			TotalItems = totalItems;
			ActionName = actionName;
			PageSize = pageSize;
			SearchQuery = searchQuery;

			if (TotalPages <= 0)
				Page = 1;
			else
				Page = Math.Clamp(page, 1, TotalPages);
		}
	}
}