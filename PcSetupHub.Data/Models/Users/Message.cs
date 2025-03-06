using PCSetupHub.Data.Models.Base;

namespace PCSetupHub.Data.Models.Users
{
	public class Message : BaseEntity
	{
		public int? SenderID { get; private set; }
		public int? ReceiverID { get; private set; }
		public User? Sender { get; private set; }
		public User? Receiver { get; private set; }
		public string Text { get; private set; } = string.Empty;
		public DateTime CreatedAt { get; private set; } = DateTime.UtcNow;
		public bool IsRead { get; private set; } = false;

		public Message() { }
		public Message(int senderID, int receiverID, string text, bool isRead = false)
		{
			SenderID = senderID;
			ReceiverID = receiverID;
			Text = text;
			IsRead = isRead;
		}

		public void SetAsRead()
			=> IsRead = true;
	}
}