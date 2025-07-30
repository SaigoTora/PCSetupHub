using PCSetupHub.Data.Models.Users;
using PCSetupHub.Data.Repositories.Base;

namespace PCSetupHub.Data.Repositories.Interfaces.Users
{
	public interface IUserRepository : IRepository<User>
	{
		#region Read
		/// <summary>
		/// Retrieves a user by login with optional related entities.
		/// </summary>
		/// <param name="login">The login of the user to find.</param>
		/// <param name="includes">The related data to include.</param>
		/// <param name="asNoTracking">Whether to disable entity tracking.</param>
		/// <returns>The matching user if found; otherwise, null.</returns>
		public Task<User?> GetByLoginAsync(string login, UserIncludes includes,
			bool asNoTracking = true);

		/// <summary>
		/// Retrieves a user by their Google ID.
		/// </summary>
		/// <param name="googleId">The external Google ID to search for.</param>
		/// <returns>The matching user if found; otherwise, null.</returns>
		public Task<User?> GetByGoogleIdAsync(string googleId);
		#endregion

		/// <summary>
		/// Checks whether a user with the given login exists.
		/// </summary>
		/// <param name="login">The login to check for existence.</param>
		/// <returns>True if a user with this login exists; otherwise, false.</returns>
		public Task<bool> ExistsByLoginAsync(string login);

		/// <summary>
		/// Checks whether a user with the given email exists.
		/// </summary>
		/// <param name="email">The email to check for existence.</param>
		/// <returns>True if a user with this email exists; otherwise, false.</returns>
		public Task<bool> ExistsByEmailAsync(string email);

		/// <summary>
		/// Checks whether a user owns the specified PC configuration.
		/// </summary>
		/// <param name="userId">The user's ID.</param>
		/// <param name="pcConfigurationId">The PC configuration ID to check.</param>
		/// <returns>True if the user owns this configuration; otherwise, false.</returns>
		public Task<bool> UserHasPcConfigurationAsync(int userId, int pcConfigurationId);

		#region Delete
		/// <summary>
		/// Fully deletes a user by login along with their related data (configurations, messages, friendships, etc.).
		/// </summary>
		/// <param name="login">The login of the user to delete.</param>
		/// <returns>True if the user was successfully deleted; otherwise, false.</returns>
		public Task<bool> FullDeleteUserAsync(string login);
		#endregion
	}
}