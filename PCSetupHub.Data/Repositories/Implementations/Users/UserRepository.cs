using Microsoft.EntityFrameworkCore;

using PCSetupHub.Data.Models.Hardware;
using PCSetupHub.Data.Models.Users;
using PCSetupHub.Data.Repositories.Base;
using PCSetupHub.Data.Repositories.Interfaces.PcConfigurations;
using PCSetupHub.Data.Repositories.Interfaces.Users;

namespace PCSetupHub.Data.Repositories.Implementations.Users
{
	public class UserRepository : BaseRepo<User>, IUserRepository
	{
		private readonly IPcConfigurationRepository _pcConfigRepository;

		public UserRepository(PcSetupContext context,
			IPcConfigurationRepository pcConfigRepository)
			: base(context)
		{
			_pcConfigRepository = pcConfigRepository;
		}
		#region Read
		public async Task<User?> GetByLoginAsync(string login, bool includeDetails)
		{
			if (includeDetails)
			{
				return await GetByLoginAsync(login,
					UserIncludes.PrivacySetting |
					UserIncludes.PcConfigurationFull |
					UserIncludes.Friendships |
					UserIncludes.Messages);
			}

			return await GetByLoginAsync(login, UserIncludes.None);
		}
		public async Task<User?> GetByLoginAsync(string login, UserIncludes includes,
			bool asNoTracking = false)
		{
			var query = Context.Users
				.AsQueryable();

			ApplyPrivacySettingInclude(ref query, includes);
			ApplyPcConfigurationInclude(ref query, includes);
			ApplyFriendshipsInclude(ref query, includes);
			ApplyMessagesInclude(ref query, includes);

			User? user = await query
				.AsSplitQuery()
				.FirstOrDefaultAsync(u => u.Login == login);

			if (includes.HasFlag(UserIncludes.PcConfigurationFull) && user != null)
			{
				PcConfiguration? pcConfig = await _pcConfigRepository
					.GetByIdAsync(user.PcConfiguration.Id, true);
				if (pcConfig != null)
					user.PcConfiguration = pcConfig;
			}

			if (asNoTracking)
				query = query.AsNoTracking();

			return user;
		}
		public async Task<User?> GetByGoogleIdAsync(string googleId)
			=> await Context.Users.FirstOrDefaultAsync(u => u.GoogleId == googleId);

		private static void ApplyPrivacySettingInclude(ref IQueryable<User> query,
			UserIncludes includes)
		{
			if (includes.HasFlag(UserIncludes.PrivacySetting))
				query = query.Include(u => u.PrivacySetting);
		}
		private static void ApplyPcConfigurationInclude(ref IQueryable<User> query,
			UserIncludes includes)
		{
			if (includes.HasFlag(UserIncludes.PcConfiguration) ||
				includes.HasFlag(UserIncludes.PcConfigurationFull))
				query = query.Include(u => u.PcConfiguration);
		}
		private static void ApplyFriendshipsInclude(ref IQueryable<User> query,
			UserIncludes includes)
		{
			if (includes.HasFlag(UserIncludes.Friendships))
				query = query
					.Include(u => u.ReceivedFriendRequests!).ThenInclude(rfr => rfr.Initiator)
					.Include(u => u.SentFriendRequests!).ThenInclude(sfr => sfr.Friend);
		}
		private static void ApplyMessagesInclude(ref IQueryable<User> query,
			UserIncludes includes)
		{
			if (includes.HasFlag(UserIncludes.Messages))
				query = query
					.Include(u => u.ReceivedMessages!)
					.Include(u => u.SentMessages!);
		}
		#endregion

		public async Task<bool> ExistsByLoginAsync(string login)
			=> await Context.Users.AnyAsync(u => u.Login == login);
		public async Task<bool> ExistsByEmailAsync(string email)
			=> await Context.Users.AnyAsync(u => u.Email == email);
		public async Task<bool> UserHasPcConfigurationAsync(int userId, int pcConfigurationId)
		{
			return await Context.PcConfigurations.AnyAsync(pc => pc.Id == pcConfigurationId
				&& pc.UserId == userId);
		}

		#region Delete
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
		#endregion
	}
}