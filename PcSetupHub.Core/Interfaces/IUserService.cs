using PCSetupHub.Core.DTOs;

namespace PCSetupHub.Core.Interfaces
{
	public interface IUserService
	{
		public Task RegisterAsync(string login, string password, string name, string email,
			int? pcConfigurationId = null, bool checkLoginUniqueness = true);
		public Task<AuthResponse> LoginAsync(string login, string password);
		public Task<bool> IsUserLoggedIn(string accessToken, string refreshToken);
	}
}