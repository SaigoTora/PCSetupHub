using PCSetupHub.Models.Base;

namespace PCSetupHub.Models.Hardware
{
	public class HardwareComponent : BaseEntity
	{
		public string Name { get; private set; } = string.Empty;
		public bool IsDefault { get; private set; }

		public HardwareComponent() { }
		public HardwareComponent(string name, bool isDefault)
		{
			Name = name;
			IsDefault = isDefault;
		}
	}
}