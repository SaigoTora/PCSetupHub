using PCSetupHub.Data.Models.Hardware;

namespace PCSetupHub.Web.ViewModels
{
	public class SearchComponentViewModel : PaginationViewModel
	{
		public int PcConfigurationId { get; set; }
		public List<HardwareComponent> Components { get; set; } = [];
		public int? CurrentComponentId { get; set; }
		public string ControllerName { get; set; } = string.Empty;

		public SearchComponentViewModel()
			: base()
		{ }
		public SearchComponentViewModel(int pcConfigurationId, string? searchQuery,
			int? currentComponentId, string controllerName, int page, int totalItems,
			string actionName = "Search", int pageSize = 30)
			: base(page, totalItems, actionName, pageSize, searchQuery)
		{
			PcConfigurationId = pcConfigurationId;
			CurrentComponentId = currentComponentId;
			ControllerName = controllerName;
		}
	}
}