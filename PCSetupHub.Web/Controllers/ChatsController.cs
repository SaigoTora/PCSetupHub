using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.AspNetCore.SignalR;

using PCSetupHub.Core.Extensions;
using PCSetupHub.Core.Interfaces;
using PCSetupHub.Data.Models.Relationships;
using PCSetupHub.Data.Models.Users;
using PCSetupHub.Data.Repositories.Base;
using PCSetupHub.Data.Repositories.Interfaces.Users;
using PCSetupHub.Web.Hubs;
using PCSetupHub.Web.ViewModels;

namespace PCSetupHub.Web.Controllers
{
	public class ChatsController : Controller
	{
		private readonly ILogger<ChatsController> _logger;
		private readonly IUserRepository _userRepository;
		private readonly IRepository<UserChats> _userChatsRepository;
		private readonly IChatRepository _chatRepository;
		private readonly IMessageRepository _messageRepository;
		private readonly IUserAccessService _userAccessService;
		private readonly IHubContext<ChatHub> _hubContext;

		public ChatsController(ILogger<ChatsController> logger,
			IUserRepository userRepository, IRepository<UserChats> userChatsRepository,
			IChatRepository chatRepository, IMessageRepository messageRepository,
			IUserAccessService userAccessService, IHubContext<ChatHub> hubContext)
		{
			_logger = logger;
			_userRepository = userRepository;
			_userChatsRepository = userChatsRepository;
			_chatRepository = chatRepository;
			_messageRepository = messageRepository;
			_userAccessService = userAccessService;
			_hubContext = hubContext;
		}

		[HttpGet("Chats/{login}")]
		[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
		public async Task<IActionResult> Index(string login)
		{
			User? user = await _userRepository.GetByLoginAsync(login, UserIncludes.PrivacySetting);
			if (user == null)
				return NotFound();
			if (user.Id != User.GetId())
				return StatusCode(403);

			Message[] messages = await _messageRepository.GetPreviewsAsync(user.Id);

			return View(messages);
		}

		[HttpGet("Chat/{chatPublicId}")]
		[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
		public async Task<IActionResult> Chat(string chatPublicId)
		{
			int? userId = User.GetId();
			if (!userId.HasValue)
				return StatusCode(403);
			if (!await _chatRepository.UserHasAccessToChat(userId.Value, chatPublicId))
				return StatusCode(403);

			User[] participants = await _chatRepository.GetChatParticipantsAsync(chatPublicId);
			bool canSendMessage = true;
			if (participants.Length == 2)
			{
				canSendMessage = await _userAccessService
					.HasAccessToMessagingAsync(participants[0], participants[1]);
			}

			int[] unreadMessageIds = await GetUnreadMessageIdsAsync(chatPublicId, userId.Value);
			if (unreadMessageIds.Length > 0)
			{
				await _messageRepository.MarkAsReadAsync(unreadMessageIds);
				await NotifyMessagesReadAsync(chatPublicId, unreadMessageIds);
			}

			Message[] messages = await _messageRepository.GetMessagesAsync(chatPublicId);
			ChatViewModel model = new(chatPublicId, participants, messages, canSendMessage);

			return View(model);
		}

		[HttpGet("Chat/Start/{login}")]
		[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
		public async Task<IActionResult> StartChat(string login)
		{
			string? currentUserLogin = User.GetLogin();
			if (string.IsNullOrEmpty(currentUserLogin))
				return StatusCode(403);
			User? user = await _userRepository.GetByLoginAsync(currentUserLogin,
				UserIncludes.PrivacySetting);
			if (user == null)
				return NotFound();

			User? targetUser = await _userRepository.GetByLoginAsync(login,
				UserIncludes.PrivacySetting);
			if (targetUser == null)
				return NotFound();

			Chat? chat = await _chatRepository.GetChatBetweenUsersAsync(user.Id, targetUser.Id);
			if (chat == null)
			{
				bool canSendMessage = await _userAccessService.HasAccessToMessagingAsync(user,
					targetUser);
				User[] participants = [user, targetUser];
				ChatViewModel model = new(participants, canSendMessage);
				return View(model);
			}

			return RedirectToAction("Chat", new { chatPublicId = chat.PublicId });
		}

		[HttpPost]
		[EnableRateLimiting("SendFirstMessage")]
		[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
		public async Task<IActionResult> SendFirstMessage(string receiverUserLogin,
			string messageText)
		{
			if (string.IsNullOrEmpty(messageText))
				return BadRequest();

			string? senderUserLogin = User.GetLogin();
			if (string.IsNullOrEmpty(senderUserLogin))
				return StatusCode(403);
			User? senderUser = await _userRepository.GetByLoginAsync(senderUserLogin,
				UserIncludes.PrivacySetting);
			if (senderUser == null)
				return NotFound();

			User? receiverUser = await _userRepository.GetByLoginAsync(receiverUserLogin,
				UserIncludes.PrivacySetting);
			if (receiverUser == null)
				return NotFound();

			if (!await _userAccessService.HasAccessToMessagingAsync(senderUser, receiverUser))
				return StatusCode(403);

			Chat chat = await _chatRepository.AddChatWithUniquePublicIdAsync();
			await _userChatsRepository.AddAsync(new UserChats(senderUser.Id, chat.Id));
			await _userChatsRepository.AddAsync(new UserChats(receiverUser.Id, chat.Id));
			await _messageRepository.AddAsync(new Message(chat.Id, senderUser.Id,
				messageText));

			return RedirectToAction("Chat", new { chatPublicId = chat.PublicId });
		}

		private async Task<int[]> GetUnreadMessageIdsAsync(string chatId, int userId)
		{
			Message[] messages = await _messageRepository.GetMessagesAsync(chatId);

			return [.. messages
				.Where(m => m.SenderId != userId && !m.IsRead)
				.Select(m => m.Id)];
		}
		private async Task NotifyMessagesReadAsync(string chatId, int[] messageIds)
		{
			if (messageIds.Length <= 0)
				return;

			foreach (int msgId in messageIds)
			{
				await _hubContext.Clients.Group(chatId)
					.SendAsync("MessageRead", msgId);
			}
		}
	}
}