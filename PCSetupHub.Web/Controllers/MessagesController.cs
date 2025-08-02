using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using PCSetupHub.Core.Extensions;
using PCSetupHub.Data.Models.Users;
using PCSetupHub.Data.Repositories.Interfaces.Users;

namespace PCSetupHub.Web.Controllers
{
	public class MessagesController : Controller
	{
		private readonly ILogger<MessagesController> _logger;
		private readonly IUserRepository _userRepository;
		private readonly IMessageRepository _messageRepository;

		public MessagesController(ILogger<MessagesController> logger,
			IUserRepository userRepository, IMessageRepository messageRepository)
		{
			_logger = logger;
			_userRepository = userRepository;
			_messageRepository = messageRepository;
		}

		[HttpGet("Messages/{login}")]
		[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
		public async Task<IActionResult> Index(string login)
		{
			User? user = await _userRepository.GetByLoginAsync(login, UserIncludes.PrivacySetting);
			if (user == null)
				return NotFound();
			if (user.Id != User.GetId())
				return StatusCode(403);

			List<Message> messages = await _messageRepository.GetPreviewsAsync(user.Id);

			return View(messages);
		}
	}
}