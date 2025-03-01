using System.Text.Json.Serialization;

using PCSetupHub.Models.Base;
using PCSetupHub.Models.Hardware;

namespace PCSetupHub.Models.Users
{
	public class User : BaseEntity
	{
		public string Login { get; private set; } = string.Empty;
		[JsonIgnore]
		public string Password { get; private set; } = string.Empty;
		public string Name { get; private set; } = string.Empty;
		public string Email { get; private set; } = string.Empty;
		public PcConfiguration? PcConfiguration { get; private set; }
		public int PcConfigurationID { get; private set; }
		public ICollection<Friendship>? ReceivedFriendRequests { get; private set; }
		public ICollection<Friendship>? SentFriendRequests { get; private set; }
		public ICollection<Comment>? WrittenComments { get; private set; }
		public ICollection<Comment>? ReceivedComments { get; private set; }
		public ICollection<Message>? SentMessages { get; private set; }
		public ICollection<Message>? ReceivedMessages { get; private set; }

		public User() { }
		public User(string login, string password, string name,
			string email, int pcConfigurationID)
		{
			Login = login;
			SetPassword(password);
			Name = name;
			Email = email;
			PcConfigurationID = pcConfigurationID;
		}

		public void SetPassword(string password)
			=> Password = BCrypt.Net.BCrypt.HashPassword(password);
		public bool VerifyPassword(string password)
			=> BCrypt.Net.BCrypt.Verify(password, Password);
	}
}