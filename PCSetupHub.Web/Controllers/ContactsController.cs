using Microsoft.AspNetCore.Mvc;

using PCSetupHub.Data.Models.Users;
using PCSetupHub.Data.Repositories.Interfaces.Users;
using PCSetupHub.Web.ViewModels;

namespace PCSetupHub.Web.Controllers
{
	public class ContactsController : Controller
	{
		private const int CONTACTS_PAGE_SIZE = 1;

		private readonly ILogger<ContactsController> _logger;
		private readonly IUserRepository _userRepository;
		private readonly IFriendshipRepository _friendshipRepository;

		public ContactsController(ILogger<ContactsController> logger,
			IUserRepository userRepository, IFriendshipRepository friendshipRepository)
		{
			_logger = logger;
			_userRepository = userRepository;
			_friendshipRepository = friendshipRepository;
		}

		[HttpGet("Friends/{login}")]
		public async Task<IActionResult> Friends(string login, string friendSearchQuery,
			int page = 1)
		{
			User? user = await _userRepository.GetByLoginAsync(login, false);
			if (user == null)
				return NotFound();

			var friendships = await _friendshipRepository.GetFriendsPageAsync(user.Id,
				friendSearchQuery, page, CONTACTS_PAGE_SIZE);
			int totalItems = await _friendshipRepository.CountFriendsAsync(user.Id,
				friendSearchQuery);

			List<User> contacts = [];
			foreach (Friendship friendship in friendships)
			{
				if (friendship.InitiatorId == user.Id && friendship.Friend != null)
					contacts.Add(friendship.Friend);
				else if (friendship.FriendId == user.Id && friendship.Initiator != null)
					contacts.Add(friendship.Initiator);
			}

			ContactsViewModel model = new(contacts, friendSearchQuery, page, totalItems, "Friends",
				CONTACTS_PAGE_SIZE, nameof(friendSearchQuery));

			return View(model);
		}
	}
}