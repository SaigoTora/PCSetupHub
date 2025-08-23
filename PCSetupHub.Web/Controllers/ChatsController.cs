using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using PCSetupHub.Core.Extensions;
using PCSetupHub.Data.Models.Relationships;
using PCSetupHub.Data.Models.Users;
using PCSetupHub.Data.Repositories.Base;
using PCSetupHub.Data.Repositories.Interfaces.Users;
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

		public ChatsController(ILogger<ChatsController> logger,
			IUserRepository userRepository, IRepository<UserChats> userChatsRepository,
			IChatRepository chatRepository, IMessageRepository messageRepository)
		{
			_logger = logger;
			_userRepository = userRepository;
			_userChatsRepository = userChatsRepository;
			_chatRepository = chatRepository;
			_messageRepository = messageRepository;
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
			Message[] messages = await _messageRepository.GetMessagesAsync(chatPublicId);
			ChatViewModel model = new(chatPublicId, participants, messages);

			return View(model);
		}

		[HttpGet("Chat/Start/{login}")]
		[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
		public async Task<IActionResult> StartChat(string login)
		{
			int? userId = User.GetId();
			if (!userId.HasValue)
				return StatusCode(403);
			User? user = await _userRepository.GetOneAsync(userId.Value);
			if (user == null)
				return NotFound();

			User? targetUser = await _userRepository.GetByLoginAsync(login, UserIncludes.None);
			if (targetUser == null)
				return NotFound();

			Chat? chat = await _chatRepository.GetChatBetweenUsersAsync(user.Id, targetUser.Id);
			if (chat == null)
			{
				User[] participants = [user, targetUser];
				ChatViewModel model = new(participants);
				return View(model);
			}

			return RedirectToAction("Chat", new { chatPublicId = chat.PublicId });
		}

		[HttpPost]
		[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
		public async Task<IActionResult> SendFirstMessage(int receiverUserId, string messageText)
		{
			if (string.IsNullOrEmpty(messageText))
				return BadRequest();

			int? senderUserId = User.GetId();
			if (!senderUserId.HasValue)
				return StatusCode(403);
			User? senderUser = await _userRepository.GetOneAsync(senderUserId.Value);
			if (senderUser == null)
				return NotFound();

			User? receiverUser = await _userRepository.GetOneAsync(receiverUserId);
			if (receiverUser == null)
				return NotFound();

			Chat chat = await _chatRepository.AddChatWithUniquePublicIdAsync();
			await _userChatsRepository.AddAsync(new UserChats(senderUserId.Value, chat.Id));
			await _userChatsRepository.AddAsync(new UserChats(receiverUserId, chat.Id));
			await _messageRepository.AddAsync(new Message(chat.Id, senderUserId.Value,
				messageText));

			return RedirectToAction("Chat", new { chatPublicId = chat.PublicId });
		}
	}
}