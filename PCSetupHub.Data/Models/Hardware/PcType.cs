using System.ComponentModel.DataAnnotations;

using PCSetupHub.Data.Models.Base;

namespace PCSetupHub.Data.Models.Hardware
{
	public class PcType : BaseEntity
	{
		[Required]
		[StringLength(255)]
		public string Name { get; private set; } = string.Empty;

		public ICollection<PcConfiguration>? PcConfigurations { get; private set; }

		public PcType() { }
		public PcType(string name)
		{
			Name = name;
		}
	}
}