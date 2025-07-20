using System.ComponentModel.DataAnnotations;

using PCSetupHub.Data.Models.Base;

namespace PCSetupHub.Data.Models.Users
{
	public class PrivacyLevel : BaseEntity
	{
		[Required]
		[StringLength(255)]
		public string Name { get; private set; } = string.Empty;

		public ICollection<User>? FollowersAccessUsers { get; private set; }
		public ICollection<User>? FollowingsAccessUsers { get; private set; }
		public ICollection<User>? MessagesAccessUsers { get; private set; }
		public ICollection<User>? PcConfigAccessUsers { get; private set; }

		public PrivacyLevel() { }
		public PrivacyLevel(string name)
		{
			Name = name;
		}
	}
}