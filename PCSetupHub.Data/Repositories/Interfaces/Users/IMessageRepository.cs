using PCSetupHub.Data.Models.Users;
using PCSetupHub.Data.Repositories.Base;

namespace PCSetupHub.Data.Repositories.Interfaces.Users
{
	public interface IMessageRepository : IRepository<Message>
	{
		public Task<List<Message>> GetPreviewsAsync(int userId);
	}
}