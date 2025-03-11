using System.Text.Json.Serialization;

using PCSetupHub.Data.Models.Base;
using PCSetupHub.Data.Models.Hardware;

namespace PCSetupHub.Data.Models.Users
{
	public class User : BaseEntity
	{
		public string Login { get; private set; } = string.Empty;
		[JsonIgnore]
		public string PasswordHash { get; private set; } = string.Empty;
		public string Name { get; private set; } = string.Empty;
		public string Email { get; private set; } = string.Empty;
		public PcConfiguration? PcConfiguration { get; private set; }
		public int PcConfigurationId { get; private set; }
		public ICollection<Friendship>? ReceivedFriendRequests { get; private set; }
		public ICollection<Friendship>? SentFriendRequests { get; private set; }
		public ICollection<Comment>? WrittenComments { get; private set; }
		public ICollection<Comment>? ReceivedComments { get; private set; }
		public ICollection<Message>? SentMessages { get; private set; }
		public ICollection<Message>? ReceivedMessages { get; private set; }

		public User() { }
		public User(string login, string passwordHash, string name,
			string email, int? pcConfigurationId)
		{
			Login = login;
			PasswordHash = passwordHash;
			Name = name;
			Email = email;

			if (pcConfigurationId.HasValue)
				PcConfigurationId = pcConfigurationId.Value;
			else
				PcConfiguration = new PcConfiguration();
		}

		public void ChangePasswordHash(string newPasswordHash)
			=> PasswordHash = newPasswordHash;
	}
}