using PCSetupHub.Data.Models.Users;

namespace PCSetupHub.Web.ViewModels
{
	public class ChatViewModel
	{
		public string ChatId { get; private set; } = string.Empty;
		public User[]? Participants { get; private set; }
		public Message[]? Messages { get; private set; }
		public bool CanSendMessage { get; private set; }
		public bool IsChatEmpty { get; private set; }

		public ChatViewModel(User[] participants)
		{
			Participants = participants;
			IsChatEmpty = true;
		}
		public ChatViewModel(string chatId, User[] participants, Message[] messages,
			bool canSendMessage)
		{
			ChatId = chatId;
			Participants = participants;
			Messages = messages;
			IsChatEmpty = messages.Length == 0;
			CanSendMessage = canSendMessage;
		}
	}
}