using Microsoft.AspNetCore.Identity;
using System.Security.Authentication;

using PCSetupHub.Core.DTOs;
using PCSetupHub.Core.Exceptions;
using PCSetupHub.Core.Interfaces;
using PCSetupHub.Data.Models.Users;
using PCSetupHub.Data.Repositories.Interfaces.Users;
using PCSetupHub.Data.Repositories.Base;
using PCSetupHub.Data.Models.Hardware;

namespace PCSetupHub.Core.Services
{
	public class UserService : IUserService
	{
		private readonly IUserRepository _userRepository;
		private readonly IRepository<PrivacySetting> _privacySettingRepository;
		private readonly IRepository<PcConfiguration> _pcConfigurationRepository;
		private readonly JwtService _jwtService;

		public UserService(IUserRepository userRepository,
			IRepository<PrivacySetting> privacySettingRepository,
			IRepository<PcConfiguration> pcConfigurationRepository,
			JwtService jwtService)
		{
			_userRepository = userRepository;
			_privacySettingRepository = privacySettingRepository;
			_pcConfigurationRepository = pcConfigurationRepository;
			_jwtService = jwtService;
		}

		public async Task RegisterAsync(string login, string password, string name, string email,
			string? description, bool checkUniqueness = true)
		{
			if (checkUniqueness && await _userRepository.ExistsByLoginAsync(login))
				throw new UserAlreadyExistsException($"User with login '{login}' already exists.");
			if (checkUniqueness && await _userRepository.ExistsByEmailAsync(email))
				throw new EmailAlreadyExistsException($"User with email " +
					$"'{email}' already exists.");

			User user = new(login, password, name, email, description);
			string passwordHash = HashPassword(user, password);
			user.PasswordHash = passwordHash;

			user = await _userRepository.AddAsync(user);
			await _privacySettingRepository.AddAsync(new PrivacySetting(user.Id));
			await _pcConfigurationRepository.AddAsync(new PcConfiguration(user.Id));
		}
		public string HashPassword(User user, string password)
			=> new PasswordHasher<User>().HashPassword(user, password);
		public bool VerifyPassword(User user, string password)
		{
			PasswordVerificationResult result = new PasswordHasher<User>()
				.VerifyHashedPassword(user, user.PasswordHash!, password);

			return result == PasswordVerificationResult.Success;
		}
		public async Task<AuthResponse> LoginAsync(string login, string password,
			bool userRememberMe)
		{
			User? user = await _userRepository.GetByLoginAsync(login, UserIncludes.Password)
				?? throw new AuthenticationException("User not found.");

			if (VerifyPassword(user, password))
			{
				string accessToken = await _jwtService.GenerateAccessTokenAsync(user,
					userRememberMe);
				string refreshToken = await _jwtService.GenerateRefreshTokenAsync();
				return new AuthResponse(accessToken, refreshToken);
			}
			else
				throw new AuthenticationException("Invalid password.");
		}
		public async Task<AuthResponse> LoginOrRegisterByGoogleIdAsync(string googleId,
			string email, string name, string? login = null)
		{
			User? user = await _userRepository.GetByGoogleIdAsync(googleId);
			if (user == null)
			{
				login ??= email;

				user = new User(login, null, name, email, null)
				{
					GoogleId = googleId
				};
				user = await _userRepository.AddAsync(user);
				await _privacySettingRepository.AddAsync(new PrivacySetting(user.Id));
				await _pcConfigurationRepository.AddAsync(new PcConfiguration(user.Id));
			}

			string accessToken = await _jwtService.GenerateAccessTokenAsync(user, true);
			string refreshToken = await _jwtService.GenerateRefreshTokenAsync();
			return new AuthResponse(accessToken, refreshToken);
		}
		public async Task<bool> IsUserLoggedInAsync(string accessToken, string refreshToken)
		{
			(bool isRefreshTokenValid, _)
				= await _jwtService.ValidateAndRefreshTokensAsync(accessToken, refreshToken);

			return isRefreshTokenValid;
		}
	}
}