using PCSetupHub.Models.Base;
using PCSetupHub.Models.Hardware;

namespace PCSetupHub.Models.Users
{
	public class User : BaseEntity
	{
		public string Login { get; private set; } = string.Empty;
		public string Password { get; private set; } = string.Empty;
		public string Name { get; private set; } = string.Empty;
		public string Email { get; private set; } = string.Empty;
		public PcConfiguration PcConfiguration { get; private set; } = new PcConfiguration();
		public int PcConfigurationID { get; private set; }
		public ICollection<Friendship> Friendships { get; private set; }
			= new HashSet<Friendship>();
		public ICollection<Comment> Comments { get; private set; }
			= new HashSet<Comment>();
		public ICollection<Message> Messages { get; private set; }
			= new HashSet<Message>();

		public User() { }
		public User(string login, string password, string name,
			string email, PcConfiguration pcConfiguration)
		{
			Login = login;
			Password = password;
			Name = name;
			Email = email;
			PcConfiguration = pcConfiguration;
		}
	}
}