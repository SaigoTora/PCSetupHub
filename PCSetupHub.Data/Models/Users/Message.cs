using PCSetupHub.Data.Models.Base;

namespace PCSetupHub.Data.Models.Users
{
	public class Message : BaseEntity
	{
		public int SenderId { get; private set; }
		public int ReceiverId { get; private set; }
		public User? Sender { get; private set; }
		public User? Receiver { get; private set; }
		public string Text { get; private set; } = string.Empty;
		public DateTime CreatedAt { get; private set; } = DateTime.UtcNow;
		public bool IsRead { get; set; } = false;

		public Message() { }
		public Message(int senderId, int receiverId, string text, bool isRead = false)
		{
			SenderId = senderId;
			ReceiverId = receiverId;
			Text = text;
			IsRead = isRead;
		}
	}
}