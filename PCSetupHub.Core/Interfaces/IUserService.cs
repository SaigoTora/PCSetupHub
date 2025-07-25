using PCSetupHub.Core.DTOs;
using PCSetupHub.Data.Models.Users;

namespace PCSetupHub.Core.Interfaces
{
	public interface IUserService
	{
		public Task RegisterAsync(string login, string password, string name, string email,
			string? description, bool checkLoginUniqueness = true);
		public string HashPassword(User user, string password);
		public bool VerifyPassword(User user, string password);
		public Task<AuthResponse> LoginAsync(string login, string password, bool userRememberMe);
		public Task<AuthResponse> LoginOrRegisterByGoogleIdAsync(string googleId, string email,
			string name, string? login = null);
		public Task<bool> IsUserLoggedInAsync(string accessToken, string refreshToken);
	}
}