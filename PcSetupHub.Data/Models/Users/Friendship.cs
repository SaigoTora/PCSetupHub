using PCSetupHub.Data.Models.Base;

namespace PCSetupHub.Data.Models.Users
{
	public class Friendship : BaseEntity
	{
		public int InitiatorId { get; private set; }
		public int FriendID { get; private set; }
		public int FriendshipStatusID { get; private set; }
		public User? Initiator { get; private set; }
		public User? Friend { get; private set; }
		public FriendshipStatus? FriendshipStatus { get; private set; }
		public DateTime CreatedAt { get; private set; } = DateTime.UtcNow;

		public Friendship() { }
		public Friendship(int initiatorId, int friendID, int friendshipStatusID)
		{
			FriendID = friendID;
			InitiatorId = initiatorId;
			FriendshipStatusID = friendshipStatusID;
		}
	}
}