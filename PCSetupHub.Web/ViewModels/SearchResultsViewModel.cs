using PCSetupHub.Data.Models.Hardware;

namespace PCSetupHub.Web.ViewModels
{
	public class SearchResultsViewModel
	{
		public int PcConfigurationId { get; set; }
		public List<HardwareComponent> Components { get; set; } = [];
		public int? CurrentComponentId { get; set; }
		public string ControllerName { get; set; } = string.Empty;

		public SearchResultsViewModel() { }
		public SearchResultsViewModel(int pcConfigurationId, List<HardwareComponent> components,
			int? currentComponentId, string controllerName)
		{
			PcConfigurationId = pcConfigurationId;
			Components = components;
			CurrentComponentId = currentComponentId;
			ControllerName = controllerName;
		}
	}
}