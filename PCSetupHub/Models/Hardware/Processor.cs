namespace PCSetupHub.Models.Hardware
{
	public class Processor : HardwareComponent
	{
		public byte CoreCount { get; private set; }
		public float CoreClock { get; private set; }
		public float? BoostClock { get; private set; }
		public int TDP { get; private set; }
		public string? Graphics { get; private set; }
		public bool SMT { get; private set; }
		public ICollection<PcConfiguration> PcConfigurations { get; private set; }
			= [];

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