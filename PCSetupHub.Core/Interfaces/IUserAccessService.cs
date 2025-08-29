using PCSetupHub.Data.Models.Users;

namespace PCSetupHub.Core.Interfaces
{
	public interface IUserAccessService
	{
		/// <summary>
		/// Checks asynchronously whether the requester has access to the target user's data
		/// according to the specified privacy level.
		/// </summary>
		/// <param name="requesterLogin">Login of the user requesting access.</param>
		/// <param name="targetLogin">Login of the target user whose data is requested.</param>
		/// <param name="level">The privacy level to check access against.</param>
		/// <returns>A task that returns true if access is allowed; otherwise false.</returns>
		Task<bool> HasAccessAsync(string requesterLogin, string targetLogin,
			PrivacyLevelType level);

		/// <summary>
		/// Checks asynchronously whether the given user has permission to send messages
		/// to the specified target user, considering both users' privacy settings.
		/// </summary>
		/// <param name="user">The user who wants to send a message.</param>
		/// <param name="targetUser">The target user who may receive the message.</param>
		/// <returns>
		/// A task that returns true if messaging is allowed; otherwise false.
		/// </returns>
		Task<bool> HasAccessToMessagingAsync(User user, User targetUser);
	}
}