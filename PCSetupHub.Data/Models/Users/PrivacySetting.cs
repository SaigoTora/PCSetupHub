using PCSetupHub.Data.Models.Base;

namespace PCSetupHub.Data.Models.Users
{
	public class PrivacySetting : BaseEntity
	{
		public User? User { get; private set; }
		public int UserId { get; private set; }

		public PrivacyLevel? FriendsAccess { get; private set; }
		public int FriendsAccessId { get; private set; } = (int)PrivacyLevelType.Everyone;

		public PrivacyLevel? FollowersAccess { get; private set; }
		public int FollowersAccessId { get; private set; } = (int)PrivacyLevelType.Everyone;

		public PrivacyLevel? FollowingsAccess { get; private set; }
		public int FollowingsAccessId { get; private set; } = (int)PrivacyLevelType.Everyone;

		public PrivacyLevel? MessagesAccess { get; private set; }
		public int MessagesAccessId { get; private set; } = (int)PrivacyLevelType.Everyone;

		public PrivacyLevel? PcConfigAccess { get; private set; }
		public int PcConfigAccessId { get; private set; } = (int)PrivacyLevelType.Everyone;

		public PrivacyLevel? CommentWritingAccess { get; private set; }
		public int CommentWritingAccessId { get; private set; } = (int)PrivacyLevelType.Everyone;

		public PrivacySetting() { }
		public PrivacySetting(int userId)
		{
			UserId = userId;
		}
		public PrivacySetting(int friendsAccessId, int followersAccessId, int followingsAccessId,
			int messagesAccessId, int pcConfigAccessId, int commentWritingAccessId)
		{
			FriendsAccessId = friendsAccessId;
			FollowersAccessId = followersAccessId;
			FollowingsAccessId = followingsAccessId;
			MessagesAccessId = messagesAccessId;
			PcConfigAccessId = pcConfigAccessId;
			CommentWritingAccessId = commentWritingAccessId;
		}

		public void SetAccessLevels(PrivacySetting settings)
		{
			FriendsAccessId = settings.FriendsAccessId;
			FollowersAccessId = settings.FollowersAccessId;
			FollowingsAccessId = settings.FollowingsAccessId;
			MessagesAccessId = settings.MessagesAccessId;
			PcConfigAccessId = settings.PcConfigAccessId;
			CommentWritingAccessId = settings.CommentWritingAccessId;
		}
	}
}