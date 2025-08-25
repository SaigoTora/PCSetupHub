using PCSetupHub.Data.Models.Users;
using PCSetupHub.Data.Repositories.Base;

namespace PCSetupHub.Data.Repositories.Interfaces.Users
{
	public interface IMessageRepository : IRepository<Message>
	{
		/// <summary>
		/// Retrieves the latest preview messages for a given user across all their chats.
		/// </summary>
		/// <param name="userId">The ID of the user to get message previews for.</param>
		/// <returns>An array of the latest messages, including participants information.</returns>
		public Task<Message[]> GetPreviewsAsync(int userId);

		/// <summary>
		/// Retrieves all messages from a chat identified by its public ID.
		/// </summary>
		/// <param name="chatPublicId">The public ID of the chat.</param>
		/// <returns>An array of messages from the chat, ordered by newest first.</returns>
		public Task<Message[]> GetMessagesAsync(string chatPublicId);

		/// <summary>
		/// Marks the specified messages as read in the database.
		/// </summary>
		/// <param name="messageIds">A collection of message IDs to mark as read.</param>
		/// <returns>A task representing the asynchronous operation.</returns>
		public Task MarkAsReadAsync(ICollection<int> messageIds);
	}
}