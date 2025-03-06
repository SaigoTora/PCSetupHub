using Microsoft.AspNetCore.Identity;

using PCSetupHub.Models.Users;
using PCSetupHub.Repositories;

namespace PCSetupHub.Services
{
	public class UserService(UserRepository userRepository, JwtService jwtService)
	{
		private readonly UserRepository _userRepository = userRepository;
		private readonly JwtService _jwtService = jwtService;

		public async Task RegisterAsync(string login, string password, string name, string email,
			int? pcConfigurationId = null)
		{
			User user = new(login, password, name, email, pcConfigurationId);
			string passwordHash = new PasswordHasher<User>().HashPassword(user, password);
			user.ChangePasswordHash(passwordHash);

			await _userRepository.AddAsync(user);
		}
		public async Task<string> LoginAsync(string login, string password)
		{
			User? user = await _userRepository.GetByLoginAsync(login)
				?? throw new UnauthorizedAccessException("User not found.");

			var result = new PasswordHasher<User>()
				.VerifyHashedPassword(user, user.PasswordHash, password);

			if (result == PasswordVerificationResult.Success)
				return _jwtService.GenerateToken(user);
			else
				throw new UnauthorizedAccessException("Invalid password.");
		}
	}
}