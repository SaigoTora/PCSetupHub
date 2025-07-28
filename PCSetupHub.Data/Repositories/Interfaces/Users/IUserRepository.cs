using PCSetupHub.Data.Models.Users;
using PCSetupHub.Data.Repositories.Base;

namespace PCSetupHub.Data.Repositories.Interfaces.Users
{
	public interface IUserRepository : IRepository<User>
	{
		public Task<User?> GetByLoginAsync(string login, bool includeDetails);
		public Task<User?> GetByLoginAsync(string login, UserIncludes includes,
			bool asNoTracking = false);
		public Task<User?> GetByGoogleIdAsync(string googleId);

		public Task<bool> ExistsByLoginAsync(string login);
		public Task<bool> ExistsByEmailAsync(string email);
		public Task<bool> UserHasPcConfigurationAsync(int userId, int pcConfigurationId);

		public Task<bool> FullDeleteUserAsync(string login);
	}
}