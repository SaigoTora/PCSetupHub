using PCSetupHub.Data.Models.Users;
using PCSetupHub.Data.Repositories.Base;

namespace PcSetupHub.Data.Repositories.Interfaces
{
	public interface IUserRepository : IRepository<User>
	{
		public Task<User?> GetByLoginAsync(string login);
	}
}