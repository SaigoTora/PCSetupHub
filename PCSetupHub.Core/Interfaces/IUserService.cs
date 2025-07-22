using PCSetupHub.Core.DTOs;

namespace PCSetupHub.Core.Interfaces
{
	public interface IUserService
	{
		public Task RegisterAsync(string login, string password, string name, string email,
			string? description, bool checkLoginUniqueness = true);
		public Task<AuthResponse> LoginAsync(string login, string password, bool userRememberMe);
		public Task<AuthResponse> LoginOrRegisterByGoogleIdAsync(string googleId, string email,
			string name);
		public Task<bool> IsUserLoggedInAsync(string accessToken, string refreshToken);
	}
}