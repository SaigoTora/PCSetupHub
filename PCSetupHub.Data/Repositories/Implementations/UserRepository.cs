using Microsoft.EntityFrameworkCore;

using PCSetupHub.Data.Repositories.Interfaces;
using PCSetupHub.Data.Models.Users;
using PCSetupHub.Data.Repositories.Base;

namespace PCSetupHub.Data.Repositories.Implementations
{
	public class UserRepository(PcSetupContext context) : BaseRepo<User>(context), IUserRepository
	{
		public async Task<User?> GetByLoginAsync(string login)
			=> await Context.Users.FirstOrDefaultAsync(u => u.Login == login);
		public async Task<User?> GetByGoogleIdAsync(string googleId)
			=> await Context.Users.FirstOrDefaultAsync(u => u.GoogleId == googleId);
		public async Task<bool> ExistsByLoginAsync(string login)
			=> await Context.Users.AnyAsync(u => u.Login == login);
		public async Task<bool> ExistsByEmailAsync(string email)
			=> await Context.Users.AnyAsync(u => u.Email == email);
	}
}