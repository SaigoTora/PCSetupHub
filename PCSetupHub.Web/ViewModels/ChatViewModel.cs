using PCSetupHub.Data.Models.Users;

namespace PCSetupHub.Web.ViewModels
{
	public class ChatViewModel
	{
		public string ChatId { get; private set; }
		public ICollection<User>? Participants { get; private set; }
		public ICollection<Message>? Messages { get; private set; }

		public ChatViewModel(string chatId, ICollection<User> participants, ICollection<Message> messages)
		{
			ChatId = chatId;
			Participants = participants;
			Messages = messages;
		}
	}
}