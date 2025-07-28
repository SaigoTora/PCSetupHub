using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;

using PCSetupHub.Core.Extensions;
using PCSetupHub.Core.Interfaces;
using PCSetupHub.Data.Models.Users;
using PCSetupHub.Data.Repositories.Base;
using PCSetupHub.Data.Repositories.Interfaces.Users;

namespace PCSetupHub.Web.Controllers
{
	public class ProfileController : Controller
	{
		private readonly ILogger<ProfileController> _logger;
		private readonly IUserRepository _userRepository;
		private readonly IRepository<Friendship> _friendshipRepository;
		private readonly IRepository<Comment> _commentRepository;
		private readonly IImageStorageService _imageStorageService;

		public ProfileController(ILogger<ProfileController> logger,
			IUserRepository userRepository, IRepository<Friendship> friendshipRepository,
			IRepository<Comment> commentRepository, IImageStorageService s3FileService)
		{
			_logger = logger;
			_userRepository = userRepository;
			_friendshipRepository = friendshipRepository;
			_commentRepository = commentRepository;
			_imageStorageService = s3FileService;
		}

		[HttpGet("Profile/{login?}")]
		[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
		public async Task<IActionResult> Index(string login, int commentPage = 1)
		{
			if (string.IsNullOrWhiteSpace(login))
				return BadRequest();

			User? user = await _userRepository.GetByLoginAsync(login, true);
			if (user == null)
				return NotFound();

			const int COMMENT_PAGE_SIZE = 6;
			ViewData["CommentCount"] = await _commentRepository
				.CountAsync(c => c.UserId == user.Id);
			ViewData["PageSize"] = COMMENT_PAGE_SIZE;
			var comments = await _commentRepository.GetPageAsync((c => c.UserId == user.Id),
				commentPage, COMMENT_PAGE_SIZE);
			user.ReceivedComments = comments;

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

			int userId = User.GetId() ?? -1;
			if (friendship.FriendId != userId)
				return StatusCode(403);

			try
			{
				friendship.ChangeStatus((FriendshipStatusType)newStatusId);
				await _friendshipRepository.UpdateAsync(friendship);
				_logger.LogInformation("User {UserId} updated friendship {FriendshipId} " +
					"to status {StatusId}", userId, id, newStatusId);
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "User {UserId} failed to update friendship {FriendshipId} " +
					"to status {StatusId}", userId, id, newStatusId);
				throw;
			}

			User? user = await _userRepository.GetOneAsync(friendship.InitiatorId);
			if (user == null)
				return NotFound();

			return RedirectToProfile(user.Login);
		}

		[HttpPost("SendFriendRequest/{id}")]
		[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
		[EnableRateLimiting("LimitPerUser")]
		public async Task<IActionResult> SendFriendRequest(int id)
		{
			int? initiatorId = User.GetId();
			if (!initiatorId.HasValue)
				return Unauthorized();

			User? user = await _userRepository.GetOneAsync(id);
			if (user == null)
				return NotFound();

			try
			{
				Friendship friendship = new(initiatorId.Value, id,
					(int)FriendshipStatusType.Pending);
				await _friendshipRepository.AddAsync(friendship);
				_logger.LogInformation("User {InitiatorId} sent a friend request " +
					"to user {TargetId}", initiatorId, id);
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "User {InitiatorId} failed to send a friend request " +
					"to user {TargetId}", initiatorId, id);
				throw;
			}

			return RedirectToProfile(user.Login);
		}

		[HttpPost("DeleteFriendship/{id}")]
		[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
		[EnableRateLimiting("LimitPerUser")]
		public async Task<IActionResult> DeleteFriendship(int id)
		{
			Friendship? friendship = await _friendshipRepository.GetOneAsync(id);
			if (friendship == null)
				return NotFound();

			int userId = User.GetId() ?? -1;
			if (friendship.InitiatorId != userId)
				return StatusCode(403);

			try
			{
				await _friendshipRepository.DeleteAsync(id);
				_logger.LogInformation("User {UserId} deleted friendship {FriendshipId}", userId,
					id);
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "User {UserId} failed to delete friendship {FriendshipId}",
					userId, id);
				throw;
			}

			User? user = await _userRepository.GetOneAsync(friendship.FriendId);
			if (user == null)
				return NotFound();

			return RedirectToProfile(user.Login);
		}

		[HttpPost("CreateComment/{profileId}")]
		[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
		[EnableRateLimiting("LimitPerUser")]
		public async Task<IActionResult> CreateComment(int profileId, string commentText)
		{
			if (string.IsNullOrWhiteSpace(commentText))
				return BadRequest();

			User? user = await _userRepository.GetOneAsync(profileId);
			if (user == null)
				return NotFound();

			int? userId = User.GetId();
			if (!userId.HasValue)
				return Unauthorized();

			try
			{
				Comment comment = new(profileId, userId.Value, commentText);
				Comment sanitizedComment = await _commentRepository.AddAsync(comment);
				sanitizedComment.ClearUser();
				sanitizedComment.ClearCommentator();

				_logger.LogInformation("Comment created: {@Comment}", sanitizedComment);
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Failed to create comment for profile {ProfileId} by user " +
					"{UserId}", profileId, userId);
				throw;
			}

			return RedirectToProfile(user.Login);
		}

		[HttpPost("DeleteComment/{id}")]
		[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
		[EnableRateLimiting("LimitPerUser")]
		public async Task<IActionResult> DeleteComment(int id)
		{
			int? userId = User.GetId();
			if (!userId.HasValue)
				return Unauthorized();

			User? user = await _userRepository.GetOneAsync(userId.Value);
			if (user == null)
				return NotFound();

			Comment? comment = await _commentRepository.GetOneAsync(id);
			if (comment == null)
				return NotFound();
			if (comment.CommentatorId != userId.Value && comment.UserId != userId.Value)
				return StatusCode(403);

			User? profileOwner = await _userRepository.GetOneAsync(comment.UserId);
			if (profileOwner == null)
				return NotFound();

			try
			{
				await _commentRepository.DeleteAsync(comment);
				comment.ClearUser();
				comment.ClearCommentator();

				_logger.LogInformation("Comment deleted (by user with id: {UserId}): {@Comment}",
					userId.Value, comment);
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Failed to delete comment (by user with id: " +
					"{DeletingUserId}) with id {CommentId} for profile {ProfileOwnerId} " +
					"(commentator id: {CommentatorId})", userId.Value, comment.Id, comment.UserId,
					comment.CommentatorId);

				throw;
			}

			return RedirectToProfile(profileOwner.Login);
		}

		[HttpPost("Profile/UploadAvatar/{login?}")]
		[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
		[EnableRateLimiting("LimitPerUser")]
		public async Task<IActionResult> UploadAvatar(string login, IFormFile file)
		{
			if (string.IsNullOrWhiteSpace(login))
				return BadRequest();

			User? user = await _userRepository.GetByLoginAsync(login, false);
			if (user == null)
				return NotFound();
			if (user.Id != User.GetId())
				return StatusCode(403);

			if (!user.HasDefaultAvatar())
			{
				_logger.LogInformation("Deleting previous avatar for user with id: {Id}", user.Id);
				await _imageStorageService.DeleteImageAsync(user.AvatarUrl);
			}

			try
			{
				_logger.LogInformation("Uploading new avatar for user with id: {Id}", user.Id);
				string imageUrl = await _imageStorageService.UploadImageAsync(file,
					Core.Settings.ImageCategory.Avatar);

				user.AvatarUrl = imageUrl;
				await _userRepository.UpdateAsync(user);
				_logger.LogInformation("Avatar successfully updated for user with id: {Id}",
					user.Id);
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Failed to change avatar for user with id: {Id}", user.Id);
				throw;
			}

			return RedirectToProfile(login);
		}

		private RedirectToActionResult RedirectToProfile(string login)
			=> RedirectToAction("Index", new { login });
	}
}