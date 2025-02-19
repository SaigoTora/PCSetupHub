using PCSetupHub.Models.Base;

namespace PCSetupHub.Models.Users
{
	public class Friendship : BaseEntity
	{
		public int UserID { get; private set; }
		public int FriendID { get; private set; }
		public int FriendshipStatusID { get; private set; }
		public User User { get; private set; } = new User();
		public User Friend { get; private set; } = new User();
		public FriendshipStatus FriendshipStatus { get; private set; } = new FriendshipStatus();
		public DateTime CreatedAt { get; private set; } = DateTime.UtcNow;

		public Friendship() { }
		public Friendship(int userID, int friendID, int friendshipStatusID)
		{
			UserID = userID;
			FriendID = friendID;
			FriendshipStatusID = friendshipStatusID;
		}
	}
}