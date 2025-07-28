using Microsoft.EntityFrameworkCore;

using PCSetupHub.Data.Models.Hardware;
using PCSetupHub.Data.Models.Users;
using PCSetupHub.Data.Repositories.Base;
using PCSetupHub.Data.Repositories.Interfaces.Users;

namespace PCSetupHub.Data.Repositories.Implementations.Users
{
	public class UserRepository(PcSetupContext context) : BaseRepo<User>(context), IUserRepository
	{
		public async Task<User?> GetByLoginAsync(string login, bool includeDetails)
		{
			var query = Context.Users
				.Include(u => u.PrivacySetting)
				.Include(u => u.PcConfiguration)
				.AsQueryable();

			if (includeDetails)
				return await query
					.Include(u => u.PcConfiguration).ThenInclude(pc => pc!.Type)
					.Include(u => u.PcConfiguration).ThenInclude(pc => pc!.Processor)
					.Include(pc => pc.PcConfiguration).ThenInclude(pc => pc!.VideoCard)
						.ThenInclude(vc => vc!.ColorVideoCards!).ThenInclude(x => x.Color)
					.Include(pc => pc.PcConfiguration).ThenInclude(pc => pc!.Motherboard)
						.ThenInclude(m => m!.ColorMotherboards!).ThenInclude(x => x.Color)
					.Include(pc => pc.PcConfiguration).ThenInclude(pc => pc!.PowerSupply)
						.ThenInclude(ps => ps!.ColorPowerSupplies!).ThenInclude(x => x.Color)
					.Include(pc => pc.PcConfiguration)
						.ThenInclude(pc => pc!.PcConfigurationSsds!).ThenInclude(pcSsd => pcSsd.Ssd)
					.Include(pc => pc.PcConfiguration)
						.ThenInclude(pc => pc!.PcConfigurationRams!).ThenInclude(pcRam => pcRam.Ram)
							.ThenInclude(ram => ram!.ColorRams!).ThenInclude(x => x.Color)
					.Include(pc => pc.PcConfiguration)
						.ThenInclude(pc => pc!.PcConfigurationHdds!).ThenInclude(pcHdd => pcHdd.Hdd)
							.ThenInclude(hdd => hdd!.ColorHdds!).ThenInclude(x => x.Color)
					.Include(u => u.ReceivedFriendRequests!).ThenInclude(rfr => rfr.Initiator)
					.Include(u => u.SentFriendRequests!).ThenInclude(sfr => sfr.Friend)
					.Include(u => u.ReceivedMessages!)
					.Include(u => u.SentMessages!)
					.AsSplitQuery()
					.FirstOrDefaultAsync(u => u.Login == login);

			return await query.FirstOrDefaultAsync(u => u.Login == login);
		}
		public async Task<User?> GetByGoogleIdAsync(string googleId)
			=> await Context.Users.FirstOrDefaultAsync(u => u.GoogleId == googleId);

		public async Task<bool> ExistsByLoginAsync(string login)
			=> await Context.Users.AnyAsync(u => u.Login == login);
		public async Task<bool> ExistsByEmailAsync(string email)
			=> await Context.Users.AnyAsync(u => u.Email == email);
		public async Task<bool> UserHasPcConfigurationAsync(int userId, int pcConfigurationId)
		{
			return await Context.PcConfigurations.AnyAsync(pc => pc.Id == pcConfigurationId
				&& pc.UserId == userId);
		}

		public async Task<bool> FullDeleteUserAsync(string login)
		{
			User? user = await GetByLoginAsync(login, true);

			if (user == null)
				return false;

			DeleteCustomHardware(user);
			await DeleteReceivedCommentsAsync(login);
			DeleteFriendships(user);
			DeleteMessages(user);

			await DeleteAsync(user.Id);

			return true;
		}

		private void DeleteCustomHardware(User user)
		{
			static bool IsNotDefault(HardwareComponent? component) =>
				component != null && !component.IsDefault;

			PcConfiguration pcConfig = user.PcConfiguration;
			if (pcConfig == null)
				return;

			if (IsNotDefault(pcConfig.Processor))
				Context.Remove(pcConfig.Processor!);
			if (IsNotDefault(pcConfig.VideoCard))
				Context.Remove(pcConfig.VideoCard!);
			if (IsNotDefault(pcConfig.Motherboard))
				Context.Remove(pcConfig.Motherboard!);
			if (IsNotDefault(pcConfig.PowerSupply))
				Context.Remove(pcConfig.PowerSupply!);

			foreach (Ram ram in pcConfig.GetRams())
				if (IsNotDefault(ram))
					Context.Remove(ram);

			foreach (Ssd ssd in pcConfig.GetSsds())
				if (IsNotDefault(ssd))
					Context.Remove(ssd);

			foreach (Hdd hdd in pcConfig.GetHdds())
				if (IsNotDefault(hdd))
					Context.Remove(hdd);
		}
		private async Task DeleteReceivedCommentsAsync(string login)
		{
			User? user = await Context.Users
				.Include(u => u.ReceivedComments)
				.FirstOrDefaultAsync(u => u.Login == login);

			if (user != null && user.ReceivedComments != null)
				foreach (Comment comment in user.ReceivedComments)
					Context.Remove(comment);
		}
		private void DeleteFriendships(User user)
		{
			if (user.SentFriendRequests != null)
				foreach (Friendship friendship in user.SentFriendRequests)
					Context.Remove(friendship);

			if (user.ReceivedFriendRequests != null)
				foreach (Friendship friendship in user.ReceivedFriendRequests)
					Context.Remove(friendship);
		}
		private void DeleteMessages(User user)
		{
			if (user.SentMessages != null)
				foreach (Message message in user.SentMessages)
					Context.Remove(message);

			if (user.ReceivedMessages != null)
				foreach (Message message in user.ReceivedMessages)
					Context.Remove(message);
		}
	}
}