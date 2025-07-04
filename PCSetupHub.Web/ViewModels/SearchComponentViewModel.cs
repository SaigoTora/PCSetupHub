using PCSetupHub.Data.Models.Hardware;

namespace PCSetupHub.Web.ViewModels
{
	public class SearchComponentViewModel : PaginationViewModel
	{
		public int PcConfigurationId { get; set; }
		public List<HardwareComponent> Components { get; set; } = [];
		public int? CurrentComponentId { get; set; }
		public string ControllerName { get; set; } = string.Empty;
		public List<int>? SelectedComponentIds { get; set; } = null;

		public SearchComponentViewModel()
			: base()
		{ }
		public SearchComponentViewModel(int pcConfigurationId, string? searchQuery,
			int? currentComponentId, string controllerName, int page, int totalItems,
			string actionName = "Search", int pageSize = 30)
			: base(page, totalItems, actionName, pageSize, "page", searchQuery,
				  $"{char.ToLower(controllerName[0]) + controllerName[1..]}SearchQuery")
		{
			PcConfigurationId = pcConfigurationId;
			CurrentComponentId = currentComponentId;
			ControllerName = controllerName;
		}
		public SearchComponentViewModel(int pcConfigurationId, string? searchQuery,
			int? currentComponentId, string controllerName, int page, int totalItems,
			List<int> selectedComponentIds, string actionName = "Search", int pageSize = 30)
			: this(pcConfigurationId, searchQuery, currentComponentId, controllerName, page,
				  totalItems, actionName, pageSize)
		{
			SelectedComponentIds = selectedComponentIds;
		}

	}
}