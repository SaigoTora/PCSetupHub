using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

using PCSetupHub.Data.Models.Base;
using PCSetupHub.Data.Models.Hardware;
using PCSetupHub.Data.Models.Relationships;

namespace PCSetupHub.Data.Models.Users
{
	public class User : BaseEntity
	{
		public const string DEFAULT_AVATAR_URL
			= "https://pcsetuphub-user-images.s3.us-east-1.amazonaws.com/avatars/default.webp";

		[StringLength(255)]
		public string Login { get; set; } = string.Empty;

		[JsonIgnore]
		public string? PasswordHash { get; set; }
		public string? GoogleId { get; set; }

		[StringLength(64)]
		public string Name { get; private set; } = string.Empty;

		[StringLength(255)]
		public string Email { get; private set; } = string.Empty;

		[StringLength(150)]
		public string? Description { get; private set; }

		[Required]
		public string AvatarUrl { get; set; } = DEFAULT_AVATAR_URL;
		public DateTime CreatedAt { get; private set; } = DateTime.UtcNow;

		public PcConfiguration PcConfiguration { get; set; } = null!;
		public PrivacySetting PrivacySetting { get; private set; } = null!;

		public ICollection<Friendship>? ReceivedFriendRequests { get; private set; }
		public ICollection<Friendship>? SentFriendRequests { get; private set; }
		public ICollection<Comment>? WrittenComments { get; private set; }
		public ICollection<Comment>? ReceivedComments { get; set; }
		public ICollection<UserChats>? UserChats { get; private set; }
		public ICollection<Message>? SentMessages { get; private set; }

		public bool HasPassword => PasswordHash != null;
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
			string email, string? description)
		{
			Login = login;
			PasswordHash = passwordHash;
			Name = name;
			Email = email;
			Description = description;
		}

		public Friendship? GetReceivedRequestFrom(int userId)
			=> ReceivedFriendRequests?.FirstOrDefault(fr => fr.InitiatorId == userId);
		public Friendship? GetSentRequestTo(int userId)
			=> SentFriendRequests?.FirstOrDefault(fr => fr.FriendId == userId);

		public bool HasDefaultAvatar() => AvatarUrl == DEFAULT_AVATAR_URL;

		public void UpdateSettings(string name, string email, string? description,
			PrivacySetting settings)
		{
			Name = name;
			Email = email;
			Description = description;
			PrivacySetting.SetAccessLevels(settings);
		}
		public void ClearUsersFromFriendships()
		{
			if (ReceivedFriendRequests != null)
				foreach (Friendship friendship in ReceivedFriendRequests)
					friendship.ResetUsers();

			if (SentFriendRequests != null)
				foreach (Friendship friendship in SentFriendRequests)
					friendship.ResetUsers();
		}
	}
}