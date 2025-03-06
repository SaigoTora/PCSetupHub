using Microsoft.EntityFrameworkCore;

using PCSetupHub.Data.Models.Users;
using PCSetupHub.Data.Repositories.Base;

namespace PCSetupHub.Data.Repositories
{
	public class UserRepository(PcSetupContext context) : BaseRepo<User>(context)
	{
		public async Task<User?> GetByLoginAsync(string login)
			=> await Context.Users.FirstOrDefaultAsync(u => u.Login == login);
	}
}