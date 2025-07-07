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
		private IQueryable<Friendship> BuildFriendshipQuery(
			Expression<Func<Friendship, bool>> filter,
			bool includeInitiator = false,
			bool includeFriend = false)
		{
			var query = Context.Friendships.AsQueryable();

			if (includeInitiator)
				query = query.Include(f => f.Initiator);

			if (includeFriend)
				query = query.Include(f => f.Friend);

			return query.Where(filter);
		}

		#region Friends
		private static Expression<Func<Friendship, bool>> GetFriendFilter(int userId,
			string searchQuery)
		{
			return f =>
				f.FriendshipStatusId == (int)FriendshipStatusType.Accepted &&
				(
					(f.InitiatorId == userId && f.Friend != null &&
						(string.IsNullOrEmpty(searchQuery)
						|| f.Friend.Login.Contains(searchQuery)
						|| f.Friend.Name.Contains(searchQuery)))
					||
					(f.FriendId == userId && f.Initiator != null &&
						(string.IsNullOrEmpty(searchQuery)
						|| f.Initiator.Login.Contains(searchQuery)
						|| f.Initiator.Name.Contains(searchQuery)))
				);
		}

		public async Task<int> CountFriendsAsync(int userId, string searchQuery)
		{
			return await BuildFriendshipQuery(GetFriendFilter(userId, searchQuery))
				.CountAsync();
		}
		public async Task<List<Friendship>> GetFriendsPageAsync(int userId, string searchQuery,
			int pageNumber, int pageSize)
		{
			return await BuildFriendshipQuery(GetFriendFilter(userId, searchQuery), true, true)
				.OrderByDescending(f => f.CreatedAt)
				.Skip((pageNumber - 1) * pageSize)
				.Take(pageSize)
				.ToListAsync();
		}
		#endregion

		#region Followers
		private static Expression<Func<Friendship, bool>> GetFollowersFilter(int userId,
			string searchQuery)
		{
			return f =>
				(f.FriendshipStatusId == (int)FriendshipStatusType.Pending
				|| f.FriendshipStatusId == (int)FriendshipStatusType.Cancelled) &&
				(
					f.FriendId == userId && f.Initiator != null &&
						(string.IsNullOrEmpty(searchQuery)
						|| f.Initiator.Login.Contains(searchQuery)
						|| f.Initiator.Name.Contains(searchQuery))
				);
		}

		public async Task<int> CountFollowersAsync(int userId, string searchQuery)
		{
			return await BuildFriendshipQuery(GetFollowersFilter(userId, searchQuery))
				.CountAsync();
		}
		public async Task<List<Friendship>> GetFollowersPageAsync(int userId, string searchQuery,
			int pageNumber, int pageSize)
		{
			return await BuildFriendshipQuery(GetFollowersFilter(userId, searchQuery),
				true, false)
				.OrderByDescending(f => f.CreatedAt)
				.Skip((pageNumber - 1) * pageSize)
				.Take(pageSize)
				.ToListAsync();
		}
		#endregion

		#region Followings
		private static Expression<Func<Friendship, bool>> GetFollowingsFilter(int userId,
			string searchQuery)
		{
			return f =>
				(f.FriendshipStatusId == (int)FriendshipStatusType.Pending
				|| f.FriendshipStatusId == (int)FriendshipStatusType.Cancelled) &&
				(
					f.InitiatorId == userId && f.Friend != null &&
						(string.IsNullOrEmpty(searchQuery)
						|| f.Friend.Login.Contains(searchQuery)
						|| f.Friend.Name.Contains(searchQuery))
				);
		}

		public async Task<int> CountFollowingsAsync(int userId, string searchQuery)
		{
			return await BuildFriendshipQuery(GetFollowingsFilter(userId, searchQuery))
				.CountAsync();
		}
		public async Task<List<Friendship>> GetFollowingsPageAsync(int userId, string searchQuery,
			int pageNumber, int pageSize)
		{
			return await BuildFriendshipQuery(GetFollowingsFilter(userId, searchQuery),
				false, true)
				.OrderByDescending(f => f.CreatedAt)
				.Skip((pageNumber - 1) * pageSize)
				.Take(pageSize)
				.ToListAsync();
		}
		#endregion
	}
}