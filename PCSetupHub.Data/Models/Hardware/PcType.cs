using PCSetupHub.Data.Models.Base;

namespace PCSetupHub.Data.Models.Hardware
{
	public class PcType : BaseEntity
	{
		public string? Name { get; private set; }
		public ICollection<PcConfiguration>? PcConfigurations { get; private set; }

		public PcType() { }
		public PcType(string name)
		{
			Name = name;
		}
	}
}