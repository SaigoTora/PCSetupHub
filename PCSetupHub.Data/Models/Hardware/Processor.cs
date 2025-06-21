using System.ComponentModel.DataAnnotations;

namespace PCSetupHub.Data.Models.Hardware
{
	public class Processor : HardwareComponent
	{
		[Range(1, byte.MaxValue, ErrorMessage = "Number of cores must be greater than 0.")]
		public byte CoreCount { get; set; }

		[Range(0.1, float.MaxValue, ErrorMessage = "Core clock must be greater than 0.")]
		public float CoreClock { get; set; }

		[Range(0.1, float.MaxValue, ErrorMessage = "Boost clock must be greater than 0.")]
		public float? BoostClock { get; set; }

		[Range(1, int.MaxValue, ErrorMessage = "TDP must be greater than 0.")]
		public int TDP { get; set; }
		[StringLength(255, MinimumLength = 3,
			ErrorMessage = "Integrated graphics must be between 3 and 255 characters long.")]
		[RegularExpression(@"^[a-zA-Z0-9\-_/:\.\+\(\)\[\],\s]+$",
			ErrorMessage = "Integrated graphics can contain Latin letters, numbers, and common symbols like " +
			"- / : . + ( )")]
		public string? Graphics { get; set; }
		public bool SMT { get; set; }
		public ICollection<PcConfiguration>? PcConfigurations { get; private set; }
		public override string DisplayName => $"{Name} {CoreCount}-Core";

		public Processor() { }
		public Processor(string name, double? price, bool isDefault, byte coreCount,
			float coreClock, float? boostClock, int tdp, string? graphics, bool smt)
			: base(name, price, isDefault)
		{
			CoreCount = coreCount;
			CoreClock = coreClock;
			BoostClock = boostClock;
			TDP = tdp;
			Graphics = graphics;
			SMT = smt;
		}
	}
}