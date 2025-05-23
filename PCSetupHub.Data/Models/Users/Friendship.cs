﻿using System.ComponentModel.DataAnnotations.Schema;

using PCSetupHub.Data.Models.Base;

namespace PCSetupHub.Data.Models.Users
{
	public class Friendship : BaseEntity
	{
		public int InitiatorId { get; private set; }
		public int FriendId { get; private set; }
		public int FriendshipStatusId { get; private set; }
		public User? Initiator { get; private set; }
		public User? Friend { get; private set; }
		public FriendshipStatus? FriendshipStatus { get; private set; }
		[Column(TypeName = "date")]
		public DateTime CreatedAt { get; private set; } = DateTime.UtcNow;
		[Column(TypeName = "date")]
		public DateTime? AcceptedAt { get; private set; }

		public Friendship() { }
		public Friendship(int initiatorId, int friendId, int friendshipStatusId)
		{
			InitiatorId = initiatorId;
			FriendId = friendId;
			FriendshipStatusId = friendshipStatusId;
		}

		public void ChangeStatus(FriendshipStatusType status)
		{
			FriendshipStatusId = (int)status;

			if (status == FriendshipStatusType.Accepted)
				SetAccepted();
			else
				AcceptedAt = null;
		}
		public void SetAccepted()
			=> AcceptedAt = DateTime.UtcNow;
	}
}