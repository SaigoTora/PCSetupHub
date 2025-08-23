using Microsoft.EntityFrameworkCore;

using PCSetupHub.Data.Models.Users;
using PCSetupHub.Data.Repositories.Base;
using PCSetupHub.Data.Repositories.Interfaces.Users;

namespace PCSetupHub.Data.Repositories.Implementations.Users
{
	public class MessageRepository : BaseRepo<Message>, IMessageRepository
	{
		public MessageRepository(PcSetupContext context)
			: base(context)
		{ }

		public async Task<Message[]> GetPreviewsAsync(int userId)
		{
			// Load chats with participants
			Chat[] chats = await Context.Chats
				.Where(c => c.UserChats!.Any(uc => uc.UserId == userId))
				.Include(c => c.UserChats!).ThenInclude(uc => uc.User)
				.AsNoTracking()
				.ToArrayAsync();

			List<Message> messages = [];

			foreach (var chat in chats)
			{
				// Load only the last unread message from others, or last message if none
				Message? lastMessage = await GetLastRelevantMessageAsync(chat.Id, userId);

				if (lastMessage != null)
				{
					// Attach participants
					lastMessage.OtherParticipants = chat.UserChats?
						.Where(uc => uc.UserId != userId)
						.Select(uc => new User(uc.User!.AvatarUrl, uc.User!.Login, uc.User!.Name))
						.ToArray();

					messages.Add(lastMessage);
				}
			}

			return [.. messages
				.OrderByDescending(m => !m.IsRead && m.SenderId != userId)
				.ThenByDescending(m => m.CreatedAt)
				.ThenByDescending(m => m.Id)];
		}
		public async Task<Message[]> GetMessagesAsync(string chatPublicId)
		{
			return await Context.Messages
				.Where(m => m.Chat!.PublicId == chatPublicId)
				.Include(m => m.Sender)
				.Include(m => m.Chat)
				.OrderByDescending(m => m.CreatedAt)
				.ThenByDescending(m => m.Id)
				.ToArrayAsync();
		}

		private Task<Message?> GetLastRelevantMessageAsync(int chatId, int userId)
		{
			return Context.Messages
				.Include(m => m.Sender)
				.Include(m => m.Chat)
				.Where(m => m.ChatId == chatId)
				.OrderByDescending(m => !m.IsRead && m.SenderId != userId)
				.ThenByDescending(m => m.CreatedAt)
				.ThenByDescending(m => m.Id)
				.FirstOrDefaultAsync();
		}
	}
}