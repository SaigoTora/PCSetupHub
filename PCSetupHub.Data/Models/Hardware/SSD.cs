using System.ComponentModel.DataAnnotations;

using PCSetupHub.Data.Models.Relationships;

namespace PCSetupHub.Data.Models.Hardware
{
	public class Ssd : HardwareComponent
	{
		[Range(8, double.MaxValue, ErrorMessage = "Capacity must be at least 8 GB.")]
		public double Capacity { get; set; }

		[Required(ErrorMessage = "Type is required.")]
		[StringLength(64, MinimumLength = 3,
			ErrorMessage = "Type must be between 3 and 64 characters long.")]
		[RegularExpression(@"^[A-Za-z0-9 ]+$",
			ErrorMessage = "Type can contain Latin letters, numbers and spaces.")]
		public string Type { get; set; } = string.Empty;

		[Range(2, int.MaxValue, ErrorMessage = "Cache must be at least 2 MB.")]
		public int? Cache { get; set; }

		[Required(ErrorMessage = "Form factor is required.")]
		[StringLength(64, MinimumLength = 3,
			ErrorMessage = "Form factor must be between 3 and 64 characters long.")]
		[RegularExpression(@"^[A-Za-z0-9 ./-]+$",
			ErrorMessage = "Form factor can contain Latin letters, numbers, spaces, dots (.), " +
			"hyphens (-) and slashes (/).")]
		public string FormFactor { get; set; } = string.Empty;

		[Required(ErrorMessage = "Interface is required.")]
		[StringLength(64, MinimumLength = 3,
			ErrorMessage = "Interface must be between 3 and 64 characters long.")]
		[RegularExpression(@"^[A-Za-z0-9 ./-]+$",
			ErrorMessage = "Interface can contain Latin letters, numbers, spaces, dots (.), " +
			"hyphens (-) and slashes (/).")]
		public string Interface { get; set; } = string.Empty;

		public ICollection<PcConfigurationSsd>? PcConfigurationSsds { get; private set; }

		public override string DisplayName => $"{Name} {Math.Round(Capacity, 2):0.##} GB";

		public Ssd() { }
		public Ssd(string name, double? price, bool isDefault, float capacity, string type,
			int? cache, string formFactor, string @interface)
			: base(name, price, isDefault)
		{
			Capacity = capacity;
			Type = type;
			Cache = cache;
			FormFactor = formFactor;
			Interface = @interface;
		}
	}
}