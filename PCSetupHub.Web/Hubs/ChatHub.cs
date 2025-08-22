using Microsoft.AspNetCore.SignalR;

using PCSetupHub.Core.DTOs;
using PCSetupHub.Data.Models.Users;
using PCSetupHub.Data.Repositories.Interfaces.Users;

namespace PCSetupHub.Web.Hubs
{
	public class ChatHub : Hub
	{
		private readonly IChatRepository _chatRepository;
		private readonly IMessageRepository _messageRepository;

		public ChatHub(IMessageRepository messageRepository, IChatRepository chatRepository)
		{
			_messageRepository = messageRepository;
			_chatRepository = chatRepository;
		}

		public async Task JoinToChat(string chatId)
		{
			await Groups.AddToGroupAsync(Context.ConnectionId, chatId);
		}
		public async Task SendMessage(ChatMessageRequest messageRequest)
		{
			if (!string.IsNullOrEmpty(messageRequest.ChatPublicId)
				&& !string.IsNullOrEmpty(messageRequest.Text))
			{
				List<Chat> chats = await _chatRepository
					.GetSomeAsync(c => c.PublicId == messageRequest.ChatPublicId);

				if (chats.Count == 1)
				{
					Chat chat = chats[0];
					Message message = new(chat.Id, messageRequest.SenderId, messageRequest.Text);
					await _messageRepository.AddAsync(message);

					ChatMessageResponse response = new(message.SenderId, message.Text,
						message.CreatedAt);

					await Clients
						.Group(messageRequest.ChatPublicId)
						.SendAsync("ReceiveMessage", response);
				}
			}
		}
	}
}