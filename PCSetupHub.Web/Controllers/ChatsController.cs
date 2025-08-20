using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using PCSetupHub.Core.Extensions;
using PCSetupHub.Data.Models.Users;
using PCSetupHub.Data.Repositories.Interfaces.Users;
using PCSetupHub.Web.ViewModels;

namespace PCSetupHub.Web.Controllers
{
	public class ChatsController : Controller
	{
		private readonly ILogger<ChatsController> _logger;
		private readonly IUserRepository _userRepository;
		private readonly IChatRepository _chatRepository;
		private readonly IMessageRepository _messageRepository;

		public ChatsController(ILogger<ChatsController> logger,
			IUserRepository userRepository, IChatRepository chatRepository,
			IMessageRepository messageRepository)
		{
			_logger = logger;
			_userRepository = userRepository;
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
	}
}