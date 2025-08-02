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

		public async Task<List<Message>> GetPreviewsAsync(int userId)
		{
			var previewIds = await Context.Messages
				.Where(m => m.SenderId == userId || m.ReceiverId == userId)
				.GroupBy(m => m.SenderId == userId ? m.ReceiverId : m.SenderId)
				.Select(g => g.OrderByDescending(m => m.CreatedAt).First().Id)
				.ToListAsync();

			var previews = await Context.Messages
				.Where(m => previewIds.Contains(m.Id))
				.Include(m => m.Sender)
				.Include(m => m.Receiver)
				.OrderBy(m => m.IsRead)
				.ThenByDescending(m => m.CreatedAt)
				.ToListAsync();

			return previews;
		}
	}
}