using PCSetupHub.Data.Models.Base;

namespace PCSetupHub.Data.Models.Users
{
	public class FriendshipStatus : BaseEntity
	{
		public string Status { get; private set; } = string.Empty;
		public ICollection<Friendship>? Friendships { get; private set; }

		public FriendshipStatus() { }
		public FriendshipStatus(string status)
		{
			Status = status;
		}
	}
}