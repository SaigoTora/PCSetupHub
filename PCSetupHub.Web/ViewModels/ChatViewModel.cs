using PCSetupHub.Data.Models.Users;

namespace PCSetupHub.Web.ViewModels
{
	public class ChatViewModel
	{
		public string ChatId { get; private set; }
		public User[]? Participants { get; private set; }
		public Message[]? Messages { get; private set; }

		public ChatViewModel(string chatId, User[] participants, Message[] messages)
		{
			ChatId = chatId;
			Participants = participants;
			Messages = messages;
		}
	}
}