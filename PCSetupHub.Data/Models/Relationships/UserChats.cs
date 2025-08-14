using PCSetupHub.Data.Models.Base;
using PCSetupHub.Data.Models.Users;

namespace PCSetupHub.Data.Models.Relationships
{
	public class UserChats : BaseEntity
	{
		public User? User { get; private set; }
		public int UserId { get; private set; }

		public Chat? Chat { get; private set; }
		public int ChatId { get; private set; }

		public UserChats() { }
		public UserChats(int userId, int chatId)
		{
			UserId = userId;
			ChatId = chatId;
		}
	}
}