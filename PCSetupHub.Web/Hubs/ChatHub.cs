using Microsoft.AspNetCore.SignalR;

using PCSetupHub.Core.DTOs;
using PCSetupHub.Core.Interfaces;
using PCSetupHub.Data.Models.Users;
using PCSetupHub.Data.Repositories.Interfaces.Users;

namespace PCSetupHub.Web.Hubs
{
	public class ChatHub : Hub
	{
		private readonly IChatRepository _chatRepository;
		private readonly IMessageRepository _messageRepository;
		private readonly IUserAccessService _userAccessService;

		public ChatHub(IMessageRepository messageRepository, IChatRepository chatRepository,
			IUserAccessService userAccessService)
		{
			_messageRepository = messageRepository;
			_chatRepository = chatRepository;
			_userAccessService = userAccessService;
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
					if (!await IsMessageAllowedAsync(chat.PublicId))
						return;

					Message message = new(chat.Id, messageRequest.SenderId, messageRequest.Text);
					await _messageRepository.AddAsync(message);

					ChatMessageResponse response = new(message.Id, message.SenderId, message.Text,
						message.CreatedAt);

					await Clients
						.Group(messageRequest.ChatPublicId)
						.SendAsync("ReceiveMessage", response);
				}
			}
		}
		public async Task MarkMessagesAsRead(string chatId, List<int> messageIds)
		{
			await _messageRepository.MarkAsReadAsync(messageIds);

			foreach (var messageId in messageIds)
				await Clients.Group(chatId).SendAsync("MessageRead", messageId);
		}

		private async Task<bool> IsMessageAllowedAsync(string chatPublicId)
		{
			User[] participants = await _chatRepository.GetChatParticipantsAsync(chatPublicId);
			if (participants.Length == 2)
			{
				if (participants[0] == null || participants[1] == null)
					return false;

				return await _userAccessService.HasAccessToMessagingAsync(participants[0],
					participants[1]);
			}

			return true;
		}
	}
}