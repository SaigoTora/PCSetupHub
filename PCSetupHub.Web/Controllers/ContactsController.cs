using Microsoft.AspNetCore.Mvc;
using System.Linq.Expressions;

using PCSetupHub.Data.Models.Users;
using PCSetupHub.Data.Repositories.Interfaces.Users;
using PCSetupHub.Web.ViewModels;

namespace PCSetupHub.Web.Controllers
{
	[Route("[Controller]")]
	public class ContactsController : Controller
	{
		private const int CONTACTS_PAGE_SIZE = 30;

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

			int totalItems = await _friendshipRepository.CountFriendsAsync(user.Id,
				friendSearchQuery);
			var friendships = await _friendshipRepository.GetFriendsPageAsync(user.Id,
				friendSearchQuery, page, CONTACTS_PAGE_SIZE);

			List<User> contacts = [];
			foreach (Friendship friendship in friendships)
			{
				if (friendship.InitiatorId == user.Id && friendship.Friend != null)
					contacts.Add(friendship.Friend);
				else if (friendship.FriendId == user.Id && friendship.Initiator != null)
					contacts.Add(friendship.Initiator);
			}

			ContactsViewModel model = new(contacts, login, friendSearchQuery, page, totalItems,
				nameof(Friends), CONTACTS_PAGE_SIZE, nameof(friendSearchQuery));

			return View(model);
		}

		[HttpGet("Followers/{login}")]
		public async Task<IActionResult> Followers(string login, string followerSearchQuery,
			int page = 1)
		{
			User? user = await _userRepository.GetByLoginAsync(login, false);
			if (user == null)
				return NotFound();

			int totalItems = await _friendshipRepository.CountFollowersAsync(user.Id,
				followerSearchQuery);
			var friendships = await _friendshipRepository.GetFollowersPageAsync(user.Id,
				followerSearchQuery, page, CONTACTS_PAGE_SIZE);

			List<User> contacts = [];
			foreach (Friendship friendship in friendships)
				if (friendship.FriendId == user.Id && friendship.Initiator != null)
					contacts.Add(friendship.Initiator);

			ContactsViewModel model = new(contacts, login, followerSearchQuery, page, totalItems,
				nameof(Followers), CONTACTS_PAGE_SIZE, nameof(followerSearchQuery));

			return View(model);
		}

		[HttpGet("Followings/{login}")]
		public async Task<IActionResult> Followings(string login, string followingSearchQuery,
			int page = 1)
		{
			User? user = await _userRepository.GetByLoginAsync(login, false);
			if (user == null)
				return NotFound();

			int totalItems = await _friendshipRepository.CountFollowingsAsync(user.Id,
				followingSearchQuery);
			var friendships = await _friendshipRepository.GetFollowingsPageAsync(user.Id,
				followingSearchQuery, page, CONTACTS_PAGE_SIZE);

			List<User> contacts = [];
			foreach (Friendship friendship in friendships)
				if (friendship.InitiatorId == user.Id && friendship.Friend != null)
					contacts.Add(friendship.Friend);

			ContactsViewModel model = new(contacts, login, followingSearchQuery, page, totalItems,
				nameof(Followings), CONTACTS_PAGE_SIZE, nameof(followingSearchQuery));

			return View(model);
		}

		public async Task<IActionResult> Search(string contactSearchQuery, int page = 1)
		{
			Expression<Func<User, bool>> filter = u
				=> u.Login.Contains(contactSearchQuery) || u.Name.Contains(contactSearchQuery);

			int totalItems = await _userRepository.CountAsync(filter);
			List<User> contacts = await _userRepository
				.GetPageAsync(filter, page, CONTACTS_PAGE_SIZE);


			ContactsViewModel model = new(contacts, string.Empty, contactSearchQuery, page,
				totalItems, nameof(Search), CONTACTS_PAGE_SIZE, nameof(contactSearchQuery));

			return View(model);
		}
	}
}