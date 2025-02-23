namespace PCSetupHub.Models.Hardware
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

		public Processor() { }
		public Processor(string name, bool isDefault, byte coreCount, float coreClock,
			float? boostClock, short tDP, string? graphics, bool sMT)
			: base(name, isDefault)
		{
			CoreCount = coreCount;
			CoreClock = coreClock;
			BoostClock = boostClock;
			TDP = tDP;
			Graphics = graphics;
			SMT = sMT;
		}
	}
}