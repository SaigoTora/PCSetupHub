namespace PCSetupHub.Core.DTOs
{
	public class ChatMessageRequest
	{
		public string ChatPublicId { get; set; } = string.Empty;
		public int SenderId { get; set; }
		public string Text { get; set; } = string.Empty;
	}
}