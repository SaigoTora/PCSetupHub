using PCSetupHub.Data.Models.Hardware;

namespace PCSetupHub.Web.ViewModels
{
	public enum FormMode
	{
		Create,
		Edit
	}

	public class PcComponentFormViewModel<TComponent> where TComponent : HardwareComponent
	{
		public TComponent? Component { get; set; }
		public FormMode Mode { get; set; }

		public PcComponentFormViewModel(TComponent? component, FormMode mode)
		{
			Component = component;
			Mode = mode;
		}
	}
}