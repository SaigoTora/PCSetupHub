using System.ComponentModel.DataAnnotations;

using PCSetupHub.Data.Models.Relationships;

namespace PCSetupHub.Data.Models.Hardware
{
	public class Motherboard : HardwareComponent
	{
		[Required(ErrorMessage = "Socket is required.")]
		[StringLength(255, MinimumLength = 3,
			ErrorMessage = "Socket must be between 3 and 255 characters long.")]
		[RegularExpression(@"^[a-zA-Z0-9\-+ ]+$",
			ErrorMessage = "Socket can contain Latin letters, numbers, spaces, " +
			"hyphens (-) and plus signs (+).")]
		public string Socket { get; set; } = string.Empty;

		[Required(ErrorMessage = "Form factor is required.")]
		[StringLength(255, MinimumLength = 3,
			ErrorMessage = "Form factor must be between 3 and 255 characters long.")]
		[RegularExpression(@"^[A-Za-z/ ]+$",
			ErrorMessage = "Form factor can contain Latin letters, spaces and slashes (/).")]
		public string FormFactor { get; set; } = string.Empty;

		[Range(2, int.MaxValue, ErrorMessage = "Max memory must be at least 2 GB.")]
		public int MaxMemory { get; set; }

		[Range(1, byte.MaxValue, ErrorMessage = "Memory slots must be at least 1.")]
		public byte MemorySlots { get; set; }

		public ICollection<PcConfiguration>? PcConfigurations { get; private set; }
		public ICollection<ColorMotherboard>? ColorMotherboards { get; set; }

		public override string DisplayName => $"{Name} {Socket}";

		public Motherboard() { }
		public Motherboard(string name, double? price, bool isDefault, string socket,
			string formFactor, int maxMemory, byte memorySlots)
			: base(name, price, isDefault)
		{
			Socket = socket;
			FormFactor = formFactor;
			MaxMemory = maxMemory;
			MemorySlots = memorySlots;
		}
	}
}