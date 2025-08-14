using System.ComponentModel.DataAnnotations;

using PCSetupHub.Data.Models.Relationships;

namespace PCSetupHub.Data.Models.Hardware
{
	public class Hdd : HardwareComponent
	{
		[Required(ErrorMessage = "Type is required.")]
		[StringLength(64, MinimumLength = 3,
			ErrorMessage = "Type must be between 3 and 64 characters long.")]
		[RegularExpression(@"^[A-Za-z/ ]+$",
			ErrorMessage = "Type can contain Latin letters, spaces and slashes (/).")]
		public string Type { get; set; } = string.Empty;

		[StringLength(255, MinimumLength = 3,
			ErrorMessage = "Interface must be between 3 and 255 characters long.")]
		[RegularExpression(@"^[A-Za-z0-9 ./\-,]+$",
			ErrorMessage = "Interface can contain Latin letters, numbers, spaces, dots (.), " +
			"hyphens (-), slashes (/) and commas (,).")]
		public string? Interface { get; set; }

		[Range(120, int.MaxValue, ErrorMessage = "Capacity must be at least 120 GB.")]
		public int Capacity { get; set; }

		public ICollection<PcConfigurationHdd>? PcConfigurationHdds { get; private set; }
		public ICollection<ColorHdd>? ColorHdds { get; set; }

		public override string DisplayName => $"{Name} {Capacity} GB";

		public Hdd() { }
		public Hdd(string name, double? price, bool isDefault,
			string type, string? @interface, int capacity)
			: base(name, price, isDefault)
		{
			Type = type;
			Interface = @interface;
			Capacity = capacity;
		}
	}
}