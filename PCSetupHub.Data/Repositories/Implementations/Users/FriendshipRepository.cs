using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

using PCSetupHub.Data.Models.Users;
using PCSetupHub.Data.Repositories.Base;
using PCSetupHub.Data.Repositories.Interfaces.Users;

namespace PCSetupHub.Data.Repositories.Implementations.Users
{
	public class FriendshipRepository(PcSetupContext context) : BaseRepo<Friendship>(context),
		IFriendshipRepository
	{
		private static Expression<Func<Friendship, bool>> GetFriendFilter(int userId,
			string searchQuery)
		{
			return f =>
				f.FriendshipStatusId == (int)FriendshipStatusType.Accepted &&
				(
					(f.InitiatorId == userId && f.Friend != null &&
						(string.IsNullOrEmpty(searchQuery) ||
						 (f.Friend.Login + " " + f.Friend.Name).Contains(searchQuery)))
					||
					(f.FriendId == userId && f.Initiator != null &&
						(string.IsNullOrEmpty(searchQuery) ||
						 (f.Initiator.Login + " " + f.Initiator.Name).Contains(searchQuery)))
				);
		}
		public async Task<int> CountFriendsAsync(int userId, string searchQuery)
		{
			return await Context.Friendships
				.Where(GetFriendFilter(userId, searchQuery))
				.CountAsync();
		}
		public async Task<List<Friendship>> GetFriendsPageAsync(int userId, string searchQuery,
			int pageNumber, int pageSize)
		{
			return await Context.Friendships
				.Include(f => f.Initiator)
				.Include(f => f.Friend)
				.Where(GetFriendFilter(userId, searchQuery))
				.OrderByDescending(f => f.CreatedAt)
				.Skip((pageNumber - 1) * pageSize)
				.Take(pageSize)
				.ToListAsync();
		}
	}
}