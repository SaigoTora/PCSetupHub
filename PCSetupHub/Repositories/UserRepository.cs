using Microsoft.EntityFrameworkCore;

using PCSetupHub.Data;
using PCSetupHub.Models.Users;
using PCSetupHub.Repositories.Base;

namespace PCSetupHub.Repositories
{
	public class UserRepository(PcSetupContext context) : BaseRepo<User>(context)
	{
		public async Task<User?> GetByLoginAsync(string login)
			=> await Context.Users.FirstOrDefaultAsync(u => u.Login == login);
	}
}