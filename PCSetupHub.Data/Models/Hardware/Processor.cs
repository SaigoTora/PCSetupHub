namespace PCSetupHub.Data.Models.Hardware
{
	public class Processor : HardwareComponent
	{
		public byte CoreCount { get; set; }
		public float CoreClock { get; set; }
		public float? BoostClock { get; set; }
		public int TDP { get; set; }
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