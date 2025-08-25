namespace PCSetupHub.Core.DTOs
{
	public class ChatMessageResponse
	{
		public int MessageId { get; set; }
		public int SenderId { get; set; }
		public string Text { get; set; } = string.Empty;
		public DateTime CreatedAt { get; set; }

		public ChatMessageResponse()
		{ }
		public ChatMessageResponse(int messageId, int senderId, string text, DateTime createdAt)
		{
			MessageId = messageId;
			SenderId = senderId;
			Text = text;
			CreatedAt = createdAt;
		}
	}
}