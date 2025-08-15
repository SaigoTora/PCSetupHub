using Microsoft.EntityFrameworkCore;

using PCSetupHub.Data.Models.Users;
using PCSetupHub.Data.Repositories.Base;
using PCSetupHub.Data.Repositories.Interfaces.Users;

namespace PCSetupHub.Data.Repositories.Implementations.Users
{
	public class ChatRepository : BaseRepo<Chat>, IChatRepository
	{
		public ChatRepository(PcSetupContext context)
			: base(context)
		{ }

		public async Task<bool> UserHasAccessToChat(int userId, string chatPublicId)
		{
			return await Context.UserChats
				.AnyAsync(uc => uc.UserId == userId && uc.Chat!.PublicId == chatPublicId);
		}
		public async Task<User[]> GetChatParticipantsAsync(string chatPublicId)
		{
			return await Context.UserChats
				.Where(uc => uc.Chat!.PublicId == chatPublicId)
				.Select(uc => uc.User!)
				.ToArrayAsync();
		}
	}
}