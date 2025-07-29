using System.ComponentModel.DataAnnotations;

using PCSetupHub.Data.Models.Base;

namespace PCSetupHub.Data.Models.Users
{
	public class PrivacyLevel : BaseEntity
	{
		[Required]
		[StringLength(255)]
		public string Name { get; private set; } = string.Empty;

		public ICollection<PrivacySetting>? FriendsAccessSettings { get; private set; }
		public ICollection<PrivacySetting>? FollowersAccessSettings { get; private set; }
		public ICollection<PrivacySetting>? FollowingsAccessSettings { get; private set; }
		public ICollection<PrivacySetting>? MessagesAccessSettings { get; private set; }
		public ICollection<PrivacySetting>? PcConfigAccessSettings { get; private set; }
		public ICollection<PrivacySetting>? CommentWritingAccessSettings { get; private set; }

		public PrivacyLevel() { }
		public PrivacyLevel(string name)
		{
			Name = name;
		}
	}
}