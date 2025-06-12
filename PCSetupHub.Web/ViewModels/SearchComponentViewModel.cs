using PCSetupHub.Data.Models.Hardware;

namespace PCSetupHub.Web.ViewModels
{
	public class SearchComponentViewModel<TComponent> where TComponent : HardwareComponent
	{
		public int PcConfigurationId { get; set; }
		public string SearchQuery { get; set; } = string.Empty;
		public List<TComponent> Components { get; set; } = [];
		public int? CurrentComponentId { get; set; } = null;
		public int PageNumber { get; set; } = 1;
		public int PageSize { get; set; } = 20;
	}
}