using Microsoft.EntityFrameworkCore;

using PCSetupHub.Data.Repositories.Interfaces;
using PCSetupHub.Data.Models.Users;
using PCSetupHub.Data.Repositories.Base;

namespace PCSetupHub.Data.Repositories.Implementations
{
	public class UserRepository(PcSetupContext context) : BaseRepo<User>(context), IUserRepository
	{
		public async Task<User?> GetByLoginAsync(string login, bool includeDetails)
		{
			var query = Context.Users.AsQueryable();

			if (includeDetails)
				return await query
					.Include(u => u.PcConfiguration).ThenInclude(pc => pc!.Processor)
					.Include(pc => pc.PcConfiguration).ThenInclude(pc => pc!.VideoCard)
					.Include(pc => pc.PcConfiguration).ThenInclude(pc => pc!.Motherboard)
					.Include(pc => pc.PcConfiguration).ThenInclude(pc => pc!.PowerSupply)
					.Include(pc => pc.PcConfiguration)
					.ThenInclude(pc => pc!.PcConfigurationSsds!).ThenInclude(pcSsd => pcSsd.Ssd)
					.Include(pc => pc.PcConfiguration)
					.ThenInclude(pc => pc!.PcConfigurationRams!).ThenInclude(pcRam => pcRam.Ram)
					.Include(pc => pc.PcConfiguration)
					.ThenInclude(pc => pc!.PcConfigurationHdds!).ThenInclude(pcHdd => pcHdd.Hdd)
					.Include(u => u.ReceivedFriendRequests!).ThenInclude(rfr => rfr.Initiator)
					.Include(u => u.SentFriendRequests!).ThenInclude(sfr => sfr.Friend)
					.FirstOrDefaultAsync(u => u.Login == login);

			return await query.FirstOrDefaultAsync(u => u.Login == login);
		}
		public async Task<User?> GetByGoogleIdAsync(string googleId)
			=> await Context.Users.FirstOrDefaultAsync(u => u.GoogleId == googleId);
		public async Task<bool> ExistsByLoginAsync(string login)
			=> await Context.Users.AnyAsync(u => u.Login == login);
		public async Task<bool> ExistsByEmailAsync(string email)
			=> await Context.Users.AnyAsync(u => u.Email == email);
	}
}