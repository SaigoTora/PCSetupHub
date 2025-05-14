using PCSetupHub.Data.Models.Hardware;

namespace PCSetupHub.Web.ViewModels
{
	public class ComponentTitleViewModel(string componentTypeName, string controllerName,
		HardwareComponent? component, bool isCurrentUserOwner)
	{
		public string ComponentTypeName { get; private set; } = componentTypeName;
		public string ControllerName { get; private set; } = controllerName;
		public HardwareComponent? Component { get; private set; } = component;
		public bool IsCurrentUserOwner { get; private set; } = isCurrentUserOwner;
	}
}