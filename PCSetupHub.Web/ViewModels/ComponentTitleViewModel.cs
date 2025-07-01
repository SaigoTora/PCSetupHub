using PCSetupHub.Data.Models.Hardware;

namespace PCSetupHub.Web.ViewModels
{
	public class ComponentTitleViewModel
	{
		public int PcConfigurationId { get; private set; }
		public string ComponentTypeName { get; private set; }
		public string ControllerName { get; private set; }
		public HardwareComponent? Component { get; private set; }
		public bool IsCurrentUserOwner { get; private set; }

		public bool ShowAddButton { get; private set; }
		public int MaxAllowedComponentCount { get; private set; }
		public int ComponentCount { get; private set; }

		public int? ComponentNumber { get; private set; }

		public ComponentTitleViewModel(int pcConfigurationId, string componentTypeName,
			string controllerName, HardwareComponent? component, bool isCurrentUserOwner)
		{
			PcConfigurationId = pcConfigurationId;
			ComponentTypeName = componentTypeName;
			ControllerName = controllerName;
			Component = component;
			IsCurrentUserOwner = isCurrentUserOwner;
		}

		public ComponentTitleViewModel(int pcConfigurationId, string componentTypeName,
			string controllerName, HardwareComponent? component, bool isCurrentUserOwner,
			bool showAddButton, int componentCount, int maxAllowedComponentCount,
			int? componentNumber = null)
			: this(pcConfigurationId, componentTypeName, controllerName, component,
				  isCurrentUserOwner)
		{
			ShowAddButton = showAddButton;
			ComponentCount = componentCount;
			MaxAllowedComponentCount = maxAllowedComponentCount;
			ComponentNumber = componentNumber;
		}
	}
}