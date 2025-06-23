using PCSetupHub.Data.Models.Hardware;

namespace PCSetupHub.Web.ViewModels
{
	public enum FormMode
	{
		Create,
		Edit
	}

	public class PcComponentFormViewModel<TComponent> where TComponent : HardwareComponent, new()
	{
		public TComponent Component { get; set; } = new();
		public FormMode Mode { get; set; }

		public List<int> SelectedColorsId { get; set; } = [];

		public PcComponentFormViewModel()
		{ }
		public PcComponentFormViewModel(TComponent component, FormMode mode)
		{
			Component = component;
			Mode = mode;
		}
	}
}