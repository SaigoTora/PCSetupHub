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
	}
}