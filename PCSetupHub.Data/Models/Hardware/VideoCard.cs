using System.ComponentModel.DataAnnotations;

using PCSetupHub.Data.Models.Relationships;

namespace PCSetupHub.Data.Models.Hardware
{
	public class VideoCard : HardwareComponent
	{
		[Required(ErrorMessage = "Chipset is required.")]
		[StringLength(255, MinimumLength = 3,
			ErrorMessage = "Chipset must be between 3 and 255 characters long.")]
		[RegularExpression(@"^[a-zA-Z0-9\-_/:\.\+\(\)\[\],\s]+$",
			ErrorMessage = "Chipset can contain Latin letters, numbers, and common symbols like " +
			"- / : . + ( )")]
		public string Chipset { get; set; } = string.Empty;

		[Range(0.1, float.MaxValue, ErrorMessage = "Memory must be greater than 0.")]
		public float Memory { get; set; }

		[Range(100, int.MaxValue, ErrorMessage = "Core clock must be at least 100 MHz.")]
		public int? CoreClock { get; set; }

		[Range(700, int.MaxValue, ErrorMessage = "Boost clock must be at least 700 MHz.")]
		public int? BoostClock { get; set; }

		[Range(60, short.MaxValue, ErrorMessage = "Length must be at least 60 mm.")]
		public short? Length { get; set; }
		public ICollection<PcConfiguration>? PcConfigurations { get; private set; }
		public ICollection<ColorVideoCard>? ColorVideoCards { get; private set; }
		public override string DisplayName => $"{Chipset} {Name} {Math.Round(Memory, 2):0.##} GB";

		public VideoCard() { }
		public VideoCard(string name, double? price, bool isDefault, string chipset, float memory,
			int? coreClock, int? boostClock, short? length)
			: base(name, price, isDefault)
		{
			Chipset = chipset;
			Memory = memory;
			CoreClock = coreClock;
			BoostClock = boostClock;
			Length = length;
		}

		public void SetColorVideoCards(ICollection<ColorVideoCard> colorVideoCards)
			=> ColorVideoCards = colorVideoCards;
	}
}