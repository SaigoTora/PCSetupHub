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

		/// <summary>
		/// Retrieves the chat between two specific users, if it exists.
		/// </summary>
		/// <param name="userId">The ID of the first user.</param>
		/// <param name="targetUserId">The ID of the second user.</param>
		/// <returns>
		/// The chat between the two users, or null if no chat exists.  
		/// If multiple chats are found, the most recent one is returned and a warning is logged.
		/// </returns>
		public Task<Chat?> GetChatBetweenUsersAsync(int userId, int targetUserId);

		/// <summary>
		/// Adds a new chat to the database with a unique public ID. 
		/// If a generated public ID already exists, it retries until a unique one is saved.
		/// </summary>
		/// <returns>The newly created chat with a unique public ID.</returns>
		public Task<Chat> AddChatWithUniquePublicIdAsync();
	}
}