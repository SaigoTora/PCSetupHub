using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;

using PCSetupHub.Core.Extensions;
using PCSetupHub.Data.Models.Users;
using PCSetupHub.Data.Repositories.Base;
using PCSetupHub.Data.Repositories.Interfaces.Users;

namespace PCSetupHub.Web.Controllers
{
	public class ProfileController(IUserRepository _userRepository,
		IRepository<Friendship> _friendshipRepository)
		: Controller
	{
		[HttpGet("Profile/{login?}")]
		[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
		public async Task<IActionResult> Index(string login)
		{
			if (string.IsNullOrWhiteSpace(login))
				return BadRequest();

			User? user = await _userRepository.GetByLoginAsync(login, true);
			if (user == null)
				return NotFound();

			return View(user);
		}

		[HttpPost("UpdateStatus/{id}")]
		[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
		[EnableRateLimiting("LimitPerUser")]
		public async Task<IActionResult> UpdateFriendshipStatus(int id, int newStatusId)
		{
			Friendship? friendship = await _friendshipRepository.GetOneAsync(id);
			if (friendship == null)
				return NotFound();
			if (friendship.FriendId != User.GetId())
				return StatusCode(403);

			friendship.ChangeStatus((FriendshipStatusType)newStatusId);
			await _friendshipRepository.UpdateAsync(friendship);

			User? user = await _userRepository.GetOneAsync(friendship.InitiatorId);
			if (user == null)
				return NotFound();

			return RedirectToAction("Index", new { login = user.Login });
		}

		[HttpPost("SendFriendRequest/{id}")]
		[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
		[EnableRateLimiting("LimitPerUser")]
		public async Task<IActionResult> SendFriendRequest(int id)
		{
			int? initiatorId = User.GetId();

			User? user = await _userRepository.GetOneAsync(id);
			if (user == null)
				return NotFound();

			if (initiatorId.HasValue)
			{
				Friendship friendship = new(initiatorId.Value, id,
					(int)FriendshipStatusType.Pending);
				await _friendshipRepository.AddAsync(friendship);
			}

			return RedirectToAction("Index", new { login = user.Login });
		}

		[HttpPost("DeleteFriendship/{id}")]
		[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
		[EnableRateLimiting("LimitPerUser")]
		public async Task<IActionResult> DeleteFriendship(int id)
		{
			Friendship? friendship = await _friendshipRepository.GetOneAsync(id);
			if (friendship == null)
				return NotFound();
			if (friendship.InitiatorId != User.GetId())
				return StatusCode(403);

			await _friendshipRepository.DeleteAsync(id);

			User? user = await _userRepository.GetOneAsync(friendship.FriendId);
			if (user == null)
				return NotFound();

			return RedirectToAction("Index", new { login = user.Login });
		}
	}
}