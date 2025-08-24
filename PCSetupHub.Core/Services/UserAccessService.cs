using PCSetupHub.Core.Interfaces;
using PCSetupHub.Data.Models.Users;
using PCSetupHub.Data.Repositories.Interfaces.Users;

namespace PCSetupHub.Core.Services
{
	public class UserAccessService : IUserAccessService
	{
		private readonly IUserRepository _userRepository;

		public UserAccessService(IUserRepository userRepository)
		{
			_userRepository = userRepository;
		}

		public async Task<bool> HasAccessAsync(string requesterLogin, string targetLogin,
			PrivacyLevelType level)
		{
			if (level == PrivacyLevelType.Everyone || requesterLogin == targetLogin)
				return true;

			UserIncludes includes = UserIncludes.PrivacySetting | UserIncludes.Friendships;
			User? target = await _userRepository.GetByLoginAsync(targetLogin, includes);
			if (target == null)
				throw new ArgumentException($"User with login '{targetLogin}' not found.",
					nameof(targetLogin));

			if (target.Friends != null)
				foreach (string login in target.Friends.Select(u => u.Login))
					if (login == requesterLogin)
					{
						if (level == PrivacyLevelType.FriendsFollowersFollowings
							|| level == PrivacyLevelType.FriendsAndFollowings
							|| level == PrivacyLevelType.FriendsOnly)
							return true;
					}

			if (target.Followings != null)
				foreach (string login in target.Followings.Select(u => u.Login))
					if (login == requesterLogin)
					{
						if (level == PrivacyLevelType.FriendsFollowersFollowings
							|| level == PrivacyLevelType.FriendsAndFollowings)
							return true;
					}

			if (target.Followers != null)
				foreach (string login in target.Followers.Select(u => u.Login))
					if (login == requesterLogin
						&& level == PrivacyLevelType.FriendsFollowersFollowings)
						return true;

			return false;
		}

		public async Task<bool> HasAccessToMessagingAsync(User user, User targetUser)
		{
			bool messageAccessGranted = await HasAccessAsync(user.Login,
				targetUser.Login, (PrivacyLevelType)targetUser.PrivacySetting.MessagesAccessId);
			if (!messageAccessGranted)
				return false;

			return await HasAccessAsync(targetUser.Login,
				user.Login, (PrivacyLevelType)user.PrivacySetting.MessagesAccessId);
		}
	}
}