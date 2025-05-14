using PCSetupHub.Data.Models.Hardware;

namespace PCSetupHub.Web.ViewModels
{
	public class ComponentTitleViewModel
	{
		public string ComponentTypeName { get; private set; }
		public string ControllerName { get; private set; }
		public HardwareComponent? Component { get; private set; }
		public bool IsCurrentUserOwner { get; private set; }
		public bool ShowAddButton { get; private set; }
		public int? ComponentNumber { get; private set; }

		public ComponentTitleViewModel(string componentTypeName, string controllerName,
			HardwareComponent? component, bool isCurrentUserOwner)
		{
			ComponentTypeName = componentTypeName;
			ControllerName = controllerName;
			Component = component;
			IsCurrentUserOwner = isCurrentUserOwner;
		}

		public ComponentTitleViewModel(string componentTypeName, string controllerName,
			HardwareComponent? component, bool isCurrentUserOwner, bool showAddButton,
			int? componentNumber = null)
			: this(componentTypeName, controllerName, component, isCurrentUserOwner)
		{
			ShowAddButton = showAddButton;
			ComponentNumber = componentNumber;
		}
	}
}