using PCSetupHub.Data.Models.Hardware;

namespace PCSetupHub.Web.ViewModels
{
	public class SearchResultsViewModel
	{
		public List<HardwareComponent> Components { get; set; } = [];
		public int? CurrentComponentId { get; set; } = null;

		public SearchResultsViewModel(List<HardwareComponent> components, int? currentComponentId)
		{
			Components = components;
			CurrentComponentId = currentComponentId;
		}
	}
}