using PCSetupHub.Core.DTOs;

namespace PCSetupHub.Core.Interfaces
{
	public interface IUserService
	{
		public Task RegisterAsync(string login, string password, string name, string email,
			string? description, int? pcConfigurationId = null, bool checkLoginUniqueness = true);
		public Task<AuthResponse> LoginAsync(string login, string password, bool userRememberMe);
		public Task<AuthResponse> LoginOrRegisterByGoogleId(string googleId, string email,
			string name);
		public Task<bool> IsUserLoggedIn(string accessToken, string refreshToken);
	}
}