using System.ComponentModel.DataAnnotations.Schema;

using PCSetupHub.Data.Models.Base;

namespace PCSetupHub.Data.Models.Users
{
	public class Friendship : BaseEntity
	{
		public User? Initiator { get; private set; }
		public int InitiatorId { get; private set; }

		public User? Friend { get; private set; }
		public int FriendId { get; private set; }

		public FriendshipStatus? Status { get; private set; }
		public int StatusId { get; private set; }

		[Column(TypeName = "date")]
		public DateTime CreatedAt { get; private set; } = DateTime.UtcNow;
		[Column(TypeName = "date")]
		public DateTime? AcceptedAt { get; private set; }

		public Friendship() { }
		public Friendship(int initiatorId, int friendId, int statusId)
		{
			InitiatorId = initiatorId;
			FriendId = friendId;
			StatusId = statusId;
		}

		public void ChangeStatus(FriendshipStatusType status)
		{
			StatusId = (int)status;

			if (status == FriendshipStatusType.Accepted)
				AcceptedAt = DateTime.UtcNow;
			else
				AcceptedAt = null;
		}
		public void ResetUsers()
		{
			Initiator = new();
			Friend = new();
		}
	}
}