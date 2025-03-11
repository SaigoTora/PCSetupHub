using Microsoft.AspNetCore.Identity;

using PCSetupHub.Core.DTOs;
using PCSetupHub.Core.Interfaces;
using PCSetupHub.Data.Models.Users;
using PCSetupHub.Data.Repositories.Interfaces;

namespace PCSetupHub.Core.Services
{
	public class UserService(IUserRepository userRepository, JwtService jwtService) : IUserService
	{
		private readonly IUserRepository _userRepository = userRepository;
		private readonly JwtService _jwtService = jwtService;

		public async Task RegisterAsync(string login, string password, string name, string email,
			int? pcConfigurationId = null)
		{
			User user = new(login, password, name, email, pcConfigurationId);
			string passwordHash = new PasswordHasher<User>().HashPassword(user, password);
			user.ChangePasswordHash(passwordHash);

			await _userRepository.AddAsync(user);
		}
		public async Task<AuthResponse> LoginAsync(string login, string password)
		{
			User? user = await _userRepository.GetByLoginAsync(login)
				?? throw new UnauthorizedAccessException("User not found.");

			PasswordVerificationResult result = new PasswordHasher<User>()
				.VerifyHashedPassword(user, user.PasswordHash, password);

			if (result == PasswordVerificationResult.Success)
			{
				return new AuthResponse(_jwtService.GenerateAccessToken(user),
					_jwtService.GenerateRefreshToken());
			}
			else
				throw new UnauthorizedAccessException("Invalid password.");
		}
		public async Task<bool> IsUserLoggedIn(string accessToken, string refreshToken)
		{
			(bool isRefreshTokenValid, _)
				= await _jwtService.ValidateAndRefreshTokensAsync(accessToken, refreshToken);

			return isRefreshTokenValid;
		}
	}
}