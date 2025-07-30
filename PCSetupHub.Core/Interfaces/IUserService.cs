using PCSetupHub.Core.DTOs;
using PCSetupHub.Data.Models.Users;

namespace PCSetupHub.Core.Interfaces
{
	public interface IUserService
	{
		/// <summary>
		/// Registers a new user with the given details.
		/// </summary>
		/// <param name="login">The user's login name.</param>
		/// <param name="password">The user's password.</param>
		/// <param name="name">The user's full name.</param>
		/// <param name="email">The user's email address.</param>
		/// <param name="description">Optional description for the user.</param>
		/// <param name="checkLoginUniqueness">If true, checks for login and email uniqueness before registering.</param>
		/// <returns>A task representing the asynchronous operation.</returns>
		public Task RegisterAsync(string login, string password, string name, string email,
			string? description, bool checkLoginUniqueness = true);

		/// <summary>
		/// Hashes the provided password for the specified user.
		/// </summary>
		/// <param name="user">The user entity.</param>
		/// <param name="password">The password to hash.</param>
		/// <returns>The hashed password string.</returns>
		public string HashPassword(User user, string password);

		/// <summary>
		/// Verifies whether the provided password matches the user's stored password hash.
		/// </summary>
		/// <param name="user">The user entity.</param>
		/// <param name="password">The password to verify.</param>
		/// <returns>True if the password is correct; otherwise, false.</returns>
		public bool VerifyPassword(User user, string password);

		/// <summary>
		/// Authenticates a user by login and password and returns tokens on success.
		/// </summary>
		/// <param name="login">The user's login name.</param>
		/// <param name="password">The user's password.</param>
		/// <param name="userRememberMe">Whether to remember the user (longer token lifespan).</param>
		/// <returns>A task with an AuthResponse containing access and refresh tokens.</returns>
		public Task<AuthResponse> LoginAsync(string login, string password, bool userRememberMe);

		/// <summary>
		/// Logs in or registers a user using their Google ID and returns tokens.
		/// </summary>
		/// <param name="googleId">The Google account ID.</param>
		/// <param name="email">The user's email from Google.</param>
		/// <param name="name">The user's name from Google.</param>
		/// <param name="login">Optional login name; defaults to email if null.</param>
		/// <returns>A task with an AuthResponse containing access and refresh tokens.</returns>
		public Task<AuthResponse> LoginOrRegisterByGoogleIdAsync(string googleId, string email,
			string name, string? login = null);

		/// <summary>
		/// Checks asynchronously if the user is currently logged in based on provided tokens.
		/// </summary>
		/// <param name="accessToken">The access token.</param>
		/// <param name="refreshToken">The refresh token.</param>
		/// <returns>A task that returns true if the tokens are valid and user is logged in; otherwise, false.</returns>
		public Task<bool> IsUserLoggedInAsync(string accessToken, string refreshToken);
	}
}