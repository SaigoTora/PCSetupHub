using PCSetupHub.Data.Models.Base;

namespace PCSetupHub.Data.Models.Users
{
	public class PrivacySetting : BaseEntity
	{
		public User? User { get; private set; }
		public int UserId { get; private set; }

		public PrivacyLevel? FollowersAccess { get; private set; }
		public int FollowersAccessId { get; private set; } = (int)PrivacyLevelType.Everyone;

		public PrivacyLevel? FollowingsAccess { get; private set; }
		public int FollowingsAccessId { get; private set; } = (int)PrivacyLevelType.Everyone;

		public PrivacyLevel? MessagesAccess { get; private set; }
		public int MessagesAccessId { get; private set; } = (int)PrivacyLevelType.Everyone;

		public PrivacyLevel? PcConfigAccess { get; private set; }
		public int PcConfigAccessId { get; private set; } = (int)PrivacyLevelType.Everyone;

		public PrivacySetting() { }
		public PrivacySetting(int userId)
		{
			UserId = userId;
		}
		public PrivacySetting(int followersAccessId, int followingsAccessId, int messagesAccessId,
			int pcConfigAccessId)
		{
			FollowersAccessId = followersAccessId;
			FollowingsAccessId = followingsAccessId;
			MessagesAccessId = messagesAccessId;
			PcConfigAccessId = pcConfigAccessId;
		}
	}
}