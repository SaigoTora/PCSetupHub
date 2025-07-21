using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

using PCSetupHub.Data.Models.Base;
using PCSetupHub.Data.Models.Hardware;

namespace PCSetupHub.Data.Models.Users
{
	public class User : BaseEntity
	{
		private const string DEFAULT_AVATAR_URL
			= "https://pcsetuphub-user-images.s3.us-east-1.amazonaws.com/avatars/default.webp";

		[StringLength(255)]
		public string Login { get; private set; } = string.Empty;

		[JsonIgnore]
		public string? PasswordHash { get; private set; }
		public string? GoogleId { get; private set; }

		[StringLength(64)]
		public string Name { get; private set; } = string.Empty;

		[StringLength(255)]
		public string Email { get; private set; } = string.Empty;

		[StringLength(150)]
		public string? Description { get; private set; }

		[Required]
		public string AvatarUrl { get; private set; } = DEFAULT_AVATAR_URL;
		public DateTime CreatedAt { get; private set; } = DateTime.UtcNow;

		public PcConfiguration? PcConfiguration { get; private set; }
		public int PcConfigurationId { get; private set; }

		public PrivacySetting PrivacySetting { get; private set; } = new();

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
							friendship.StatusId == (int)FriendshipStatusType.Accepted)
							friends.Add(friendship.Initiator);
				}
				if (SentFriendRequests != null)
				{
					foreach (Friendship friendship in SentFriendRequests)
						if (friendship.Friend != null &&
							friendship.StatusId == (int)FriendshipStatusType.Accepted)
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
							(friendship.StatusId == (int)FriendshipStatusType.Pending ||
							friendship.StatusId == (int)FriendshipStatusType.Cancelled))
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
							(friendship.StatusId == (int)FriendshipStatusType.Pending ||
							friendship.StatusId == (int)FriendshipStatusType.Cancelled))
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

		public void SetPasswordHash(string newPasswordHash)
			=> PasswordHash = newPasswordHash;
		public void SetGoogleId(string googleId)
			=> GoogleId = googleId;
		public void SetReceivedComments(ICollection<Comment> receivedComments)
			=> ReceivedComments = receivedComments;

		public Friendship? GetReceivedRequestFrom(int userId)
			=> ReceivedFriendRequests?.FirstOrDefault(fr => fr.InitiatorId == userId);
		public Friendship? GetSentRequestTo(int userId)
			=> SentFriendRequests?.FirstOrDefault(fr => fr.FriendId == userId);

		public bool HasDefaultAvatar() => AvatarUrl == DEFAULT_AVATAR_URL;
		public void SetAvatarUrl(string avatarUrl) => AvatarUrl = avatarUrl;
	}
}