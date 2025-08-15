using PCSetupHub.Data.Models.Users;

namespace PCSetupHub.Web.ViewModels
{
	public class ChatViewModel
	{
		public ICollection<User>? Participants { get; private set; }
		public ICollection<Message>? Messages { get; private set; }

		public ChatViewModel(ICollection<User> participants, ICollection<Message> messages)
		{
			Participants = participants;
			Messages = messages;
		}
	}
}