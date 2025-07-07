using PCSetupHub.Data.Models.Users;
using PCSetupHub.Data.Repositories.Base;

namespace PCSetupHub.Data.Repositories.Interfaces.Users
{
	public interface IFriendshipRepository : IRepository<Friendship>
	{
		Task<int> CountFriendsAsync(int userId, string searchQuery);
		Task<List<Friendship>> GetFriendsPageAsync(int userId, string searchQuery, int pageNumber,
			int pageSize);

		Task<int> CountFollowersAsync(int userId, string searchQuery);
		Task<List<Friendship>> GetFollowersPageAsync(int userId, string searchQuery,
			int pageNumber, int pageSize);

		Task<int> CountFollowingsAsync(int userId, string searchQuery);
		Task<List<Friendship>> GetFollowingsPageAsync(int userId, string searchQuery,
			int pageNumber, int pageSize);
	}
}