using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

using PCSetupHub.Data.Models.Base;

namespace PCSetupHub.Data.Models.Hardware
{
	public class HardwareComponent : BaseEntity
	{
		[Required(ErrorMessage = "Name is required.")]
		[StringLength(255, MinimumLength = 3,
			ErrorMessage = "Name must be between 3 and 255 characters long.")]

		[RegularExpression(@"^[a-zA-Z0-9\-_/:\.\+\(\)\[\],\s]+$",
			ErrorMessage = "Name can contain Latin letters, numbers, and common symbols like " +
			"- / : . + ( )")]
		public string Name { get; set; } = string.Empty;

		[Column(TypeName = "money")]
		[Range(0.01, double.MaxValue, ErrorMessage = "Price must be greater than 0.")]
		public double? Price { get; set; }

		public bool IsDefault { get; set; }
		public virtual string DisplayName => Name;

		public HardwareComponent() { }
		public HardwareComponent(string name, double? price, bool isDefault)
		{
			Name = name;
			Price = price;
			IsDefault = isDefault;
		}
	}
}