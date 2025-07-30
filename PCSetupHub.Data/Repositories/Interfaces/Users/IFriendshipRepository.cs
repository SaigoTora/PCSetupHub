using PCSetupHub.Data.Models.Users;
using PCSetupHub.Data.Repositories.Base;

namespace PCSetupHub.Data.Repositories.Interfaces.Users
{
	public interface IFriendshipRepository : IRepository<Friendship>
	{
		#region Friends
		/// <summary>
		/// Counts accepted friends of the specified user filtered by an optional search query.
		/// </summary>
		/// <param name="userId">ID of the user whose friends to count.</param>
		/// <param name="searchQuery">Search string to filter friends by login or name. Can be empty.</param>
		/// <returns>The number of accepted friends matching the search criteria.</returns>
		Task<int> CountFriendsAsync(int userId, string searchQuery);

		/// <summary>
		/// Retrieves a paginated list of accepted friends for the specified user,
		/// filtered by an optional search query.
		/// </summary>
		/// <param name="userId">ID of the user whose friends to retrieve.</param>
		/// <param name="searchQuery">Search string to filter friends by login or name. Can be empty.</param>
		/// <param name="pageNumber">The page number starting from 1.</param>
		/// <param name="pageSize">The number of items per page.</param>
		/// <returns>A list of accepted friends matching the criteria for the requested page.</returns>
		Task<List<Friendship>> GetFriendsPageAsync(int userId, string searchQuery, int pageNumber,
			int pageSize);
		#endregion

		#region Followers
		/// <summary>
		/// Counts pending or cancelled followers of the specified user,
		/// filtered by an optional search query.
		/// </summary>
		/// <param name="userId">ID of the user whose followers to count.</param>
		/// <param name="searchQuery">Search string to filter followers by login or name. Can be empty.</param>
		/// <returns>The number of followers matching the search criteria.</returns>
		Task<int> CountFollowersAsync(int userId, string searchQuery);

		/// <summary>
		/// Retrieves a paginated list of pending or cancelled followers for the specified user,
		/// filtered by an optional search query.
		/// </summary>
		/// <param name="userId">ID of the user whose followers to retrieve.</param>
		/// <param name="searchQuery">Search string to filter followers by login or name. Can be empty.</param>
		/// <param name="pageNumber">The page number starting from 1.</param>
		/// <param name="pageSize">The number of items per page.</param>
		/// <returns>A list of followers matching the criteria for the requested page.</returns>
		Task<List<Friendship>> GetFollowersPageAsync(int userId, string searchQuery,
			int pageNumber, int pageSize);
		#endregion

		#region Followings
		/// <summary>
		/// Counts pending or cancelled followings of the specified user,
		/// filtered by an optional search query.
		/// </summary>
		/// <param name="userId">ID of the user whose followings to count.</param>
		/// <param name="searchQuery">Search string to filter followings by login or name. Can be empty.</param>
		/// <returns>The number of followings matching the search criteria.</returns>
		Task<int> CountFollowingsAsync(int userId, string searchQuery);

		/// <summary>
		/// Retrieves a paginated list of pending or cancelled followings for the specified user,
		/// filtered by an optional search query.
		/// </summary>
		/// <param name="userId">ID of the user whose followings to retrieve.</param>
		/// <param name="searchQuery">Search string to filter followings by login or name. Can be empty.</param>
		/// <param name="pageNumber">The page number starting from 1.</param>
		/// <param name="pageSize">The number of items per page.</param>
		/// <returns>A list of followings matching the criteria for the requested page.</returns>
		Task<List<Friendship>> GetFollowingsPageAsync(int userId, string searchQuery,
			int pageNumber, int pageSize);
		#endregion
	}
}