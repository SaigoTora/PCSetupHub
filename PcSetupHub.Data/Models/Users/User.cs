using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

using PCSetupHub.Data.Models.Base;
using PCSetupHub.Data.Models.Hardware;

namespace PCSetupHub.Data.Models.Users
{
	public class User : BaseEntity
	{
		[StringLength(254)]
		public string Login { get; private set; } = string.Empty;
		[JsonIgnore]
		public string? PasswordHash { get; private set; }
		public string? GoogleId { get; private set; }
		[StringLength(64)]
		public string Name { get; private set; } = string.Empty;
		[StringLength(254)]
		public string Email { get; private set; } = string.Empty;
		[StringLength(150)]
		public string? Description { get; private set; }
		public PcConfiguration? PcConfiguration { get; private set; }
		public int PcConfigurationId { get; private set; }
		public ICollection<Friendship>? ReceivedFriendRequests { get; private set; }
		public ICollection<Friendship>? SentFriendRequests { get; private set; }
		public ICollection<Comment>? WrittenComments { get; private set; }
		public ICollection<Comment>? ReceivedComments { get; private set; }
		public ICollection<Message>? SentMessages { get; private set; }
		public ICollection<Message>? ReceivedMessages { get; private set; }

		public ICollection<User>? Friends
		{
			get
			{
				ICollection<User> friends = [];

				if (ReceivedFriendRequests != null)
				{
					foreach (Friendship friendship in ReceivedFriendRequests)
						if (friendship.Initiator != null &&
							friendship.FriendshipStatusId == (int)FriendshipStatusType.Accepted)
							friends.Add(friendship.Initiator);
				}
				if (SentFriendRequests != null)
				{
					foreach (Friendship friendship in SentFriendRequests)
						if (friendship.Friend != null &&
							friendship.FriendshipStatusId == (int)FriendshipStatusType.Accepted)
							friends.Add(friendship.Friend);
				}

				return friends;
			}
		}
		public ICollection<User>? Followers
		{
			get
			{
				ICollection<User> followers = [];

				if (ReceivedFriendRequests != null)
				{
					foreach (Friendship friendship in ReceivedFriendRequests)
						if (friendship.Initiator != null &&
							(friendship.FriendshipStatusId == (int)FriendshipStatusType.Pending ||
							friendship.FriendshipStatusId == (int)FriendshipStatusType.Cancelled))
							followers.Add(friendship.Initiator);
				}

				return followers;
			}
		}
		public ICollection<User>? Followings
		{
			get
			{
				ICollection<User> followings = [];

				if (SentFriendRequests != null)
				{
					foreach (Friendship friendship in SentFriendRequests)
						if (friendship.Friend != null &&
							(friendship.FriendshipStatusId == (int)FriendshipStatusType.Pending ||
							friendship.FriendshipStatusId == (int)FriendshipStatusType.Cancelled))
							followings.Add(friendship.Friend);
				}

				return followings;
			}
		}

		public User() { }
		public User(string login, string? passwordHash, string name,
			string email, string? description, int? pcConfigurationId)
		{
			Login = login;
			PasswordHash = passwordHash;
			Name = name;
			Email = email;
			Description = description;

			if (pcConfigurationId.HasValue)
				PcConfigurationId = pcConfigurationId.Value;
			else
				PcConfiguration = new PcConfiguration();
		}

		public void ChangePasswordHash(string newPasswordHash)
			=> PasswordHash = newPasswordHash;
		public void ChangeGoogleId(string googleId)
			=> GoogleId = googleId;

		public Friendship? GetReceivedRequestFrom(int userId)
			=> ReceivedFriendRequests?.FirstOrDefault(fr => fr.InitiatorId == userId);
		public Friendship? GetSentRequestTo(int userId)
			=> SentFriendRequests?.FirstOrDefault(fr => fr.FriendId == userId);
	}
}