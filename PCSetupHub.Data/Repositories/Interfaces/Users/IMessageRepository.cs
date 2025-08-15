using PCSetupHub.Data.Models.Users;
using PCSetupHub.Data.Repositories.Base;

namespace PCSetupHub.Data.Repositories.Interfaces.Users
{
	public interface IMessageRepository : IRepository<Message>
	{
		public Task<Message[]> GetPreviewsAsync(int userId);
	}
}