using PCSetupHub.Models.Base;

namespace PCSetupHub.Models.Users
{
	public class FriendshipStatus : BaseEntity
	{
		public string Status { get; private set; } = string.Empty;
		public ICollection<Friendship> Friendships { get; private set; }
			= new HashSet<Friendship>();

		public FriendshipStatus() { }
		public FriendshipStatus(string status)
		{
			Status = status;
		}
	}
}