using PCSetupHub.Data.Models.Base;
using PCSetupHub.Data.Models.Relationships;

namespace PCSetupHub.Data.Models.Users
{
	public class Chat : BaseEntity
	{
		public string PublicId { get; private set; } = Guid.NewGuid().ToString();
		public DateTime CreatedAt { get; private set; } = DateTime.UtcNow;

		public ICollection<UserChats>? UserChats { get; private set; }
		public ICollection<Message>? Messages { get; private set; }
	}
}