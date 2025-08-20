namespace PCSetupHub.Core.DTOs
{
	public class ChatMessageResponse
	{
		public int SenderId { get; set; }
		public string Text { get; set; } = string.Empty;
		public DateTime CreatedAt { get; set; }

		public ChatMessageResponse()
		{ }
		public ChatMessageResponse(int senderId, string text, DateTime createdAt)
		{
			SenderId = senderId;
			Text = text;
			CreatedAt = createdAt;
		}
	}
}