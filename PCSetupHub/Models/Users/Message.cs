using PCSetupHub.Models.Base;

namespace PCSetupHub.Models.Users
{
	public class Message : BaseEntity
	{
		public int SenderID { get; private set; }
		public int ReceiverID { get; private set; }
		public User? Sender { get; private set; }
		public User? Receiver { get; private set; }
		public string Text { get; private set; } = string.Empty;
		public DateTime CreatedAt { get; private set; } = DateTime.UtcNow;
		public bool IsRead { get; private set; } = false;

		public Message() { }
		public Message(int senderID, int receiverID, string text, bool isRead)
		{
			SenderID = senderID;
			ReceiverID = receiverID;
			Text = text;
			IsRead = isRead;
		}
	}
}