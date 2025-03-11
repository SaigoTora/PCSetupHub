using PCSetupHub.Core.DTOs;

namespace PCSetupHub.Core.Interfaces
{
	public interface IUserService
	{
		public Task RegisterAsync(string login, string password, string name, string email,
			int? pcConfigurationId = null);
		public Task<AuthResponse> LoginAsync(string login, string password);
	}
}