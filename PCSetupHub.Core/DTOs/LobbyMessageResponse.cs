namespace PCSetupHub.Core.DTOs
{
	public class LobbyMessageResponse : ChatMessageResponse
	{
		public string ChatPublicId { get; set; } = string.Empty;
		public string SenderAvatarUrl { get; set; } = string.Empty;
		public string SenderLogin { get; set; } = string.Empty;
		public string SenderName { get; set; } = string.Empty;

		public LobbyMessageResponse(string chatPublicId, int messageId, int senderId,
			string senderAvatarUrl, string senderLogin, string senderName, string text,
			DateTime createdAt)
			: base(messageId, senderId, text, createdAt)
		{
			ChatPublicId = chatPublicId;
			SenderAvatarUrl = senderAvatarUrl;
			SenderLogin = senderLogin;
			SenderName = senderName;
		}
	}
}