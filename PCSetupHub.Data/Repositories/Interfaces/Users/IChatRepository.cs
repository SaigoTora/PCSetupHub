using PCSetupHub.Data.Models.Users;
using PCSetupHub.Data.Repositories.Base;

namespace PCSetupHub.Data.Repositories.Interfaces.Users
{
	public interface IChatRepository : IRepository<Chat>
	{
		/// <summary>
		/// Checks if a user has access to a chat identified by its public ID.
		/// </summary>
		/// <param name="userId">The ID of the user to check access for.</param>
		/// <param name="chatPublicId">The public ID of the chat.</param>
		/// <returns>True if the user has access; otherwise, false.</returns>
		public Task<bool> UserHasAccessToChat(int userId, string chatPublicId);

		/// <summary>
		/// Retrieves all participants of a chat by its public ID.
		/// </summary>
		/// <param name="chatPublicId">The public ID of the chat.</param>
		/// <returns>An array of users participating in the chat.</returns>
		public Task<User[]> GetChatParticipantsAsync(string chatPublicId);
	}
}