using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

using PCSetupHub.Data.Models.Users;
using PCSetupHub.Data.Repositories.Base;
using PCSetupHub.Data.Repositories.Interfaces.Users;

namespace PCSetupHub.Data.Repositories.Implementations.Users
{
	public class ChatRepository : BaseRepo<Chat>, IChatRepository
	{
		private readonly ILogger<ChatRepository> _logger;

		public ChatRepository(PcSetupContext context, ILogger<ChatRepository> logger)
			: base(context)
		{
			_logger = logger;
		}

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
		public async Task<Chat?> GetChatBetweenUsersAsync(int userId, int targetUserId)
		{
			List<Chat> chats = await Context.Chats
				.Where(c => c.UserChats!.Count == 2 &&
							c.UserChats.Any(uc => uc.UserId == userId) &&
							c.UserChats.Any(uc => uc.UserId == targetUserId))
				.ToListAsync();

			if (chats.Count > 1)
			{
				_logger.LogWarning("Multiple chats found between users {UserId} and {TargetUserId}",
					userId, targetUserId);

				return chats.OrderByDescending(c => c.Id).First();
			}

			return chats.FirstOrDefault();
		}
		public async Task<Chat> AddChatWithUniquePublicIdAsync()
		{
			while (true)
			{
				Chat newChat = new();

				try
				{
					return await AddAsync(newChat);
				}
				catch (DbUpdateException)
				{
					Context.Entry(newChat).State = EntityState.Detached;
					continue;
				}
			}
		}
	}
}