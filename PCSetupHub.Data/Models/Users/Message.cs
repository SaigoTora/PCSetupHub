using PCSetupHub.Data.Models.Base;

namespace PCSetupHub.Data.Models.Users
{
	public class Message : BaseEntity
	{
		public Chat? Chat { get; private set; }
		public int ChatId { get; private set; }

		public User? Sender { get; private set; }
		public int SenderId { get; private set; }

		public string Text { get; private set; } = string.Empty;
		public DateTime CreatedAt { get; private set; } = DateTime.UtcNow;
		public bool IsRead { get; set; } = false;

		public Message() { }
		public Message(int chatId, int senderId, string text, bool isRead = false)
		{
			ChatId = chatId;
			SenderId = senderId;
			Text = text;
			IsRead = isRead;
		}
	}
}