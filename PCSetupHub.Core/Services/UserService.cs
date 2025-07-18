﻿using Microsoft.AspNetCore.Identity;
using System.Security.Authentication;

using PCSetupHub.Core.DTOs;
using PCSetupHub.Core.Exceptions;
using PCSetupHub.Core.Interfaces;
using PCSetupHub.Data.Models.Users;
using PCSetupHub.Data.Repositories.Interfaces.Users;

namespace PCSetupHub.Core.Services
{
	public class UserService : IUserService
	{
		private readonly IUserRepository _userRepository;
		private readonly JwtService _jwtService;

		public UserService(IUserRepository userRepository, JwtService jwtService)
		{
			_userRepository = userRepository;
			_jwtService = jwtService;
		}

		public async Task RegisterAsync(string login, string password, string name, string email,
			string? description, int? pcConfigurationId = null, bool checkUniqueness = true)
		{
			if (checkUniqueness && await _userRepository.ExistsByLoginAsync(login))
				throw new UserAlreadyExistsException($"User with login '{login}' already exists.");
			if (checkUniqueness && await _userRepository.ExistsByEmailAsync(email))
				throw new EmailAlreadyExistsException($"User with email " +
					$"'{email}' already exists.");

			User user = new(login, password, name, email, description, pcConfigurationId);
			string passwordHash = new PasswordHasher<User>().HashPassword(user, password);
			user.SetPasswordHash(passwordHash);

			await _userRepository.AddAsync(user);
		}
		public async Task<AuthResponse> LoginAsync(string login, string password,
			bool userRememberMe)
		{
			User? user = await _userRepository.GetByLoginAsync(login, false)
				?? throw new AuthenticationException("User not found.");

			PasswordVerificationResult result = new PasswordHasher<User>()
				.VerifyHashedPassword(user, user.PasswordHash!, password);

			if (result == PasswordVerificationResult.Success)
			{
				string accessToken = await _jwtService.GenerateAccessTokenAsync(user,
					userRememberMe);
				string refreshToken = await _jwtService.GenerateRefreshTokenAsync();
				return new AuthResponse(accessToken, refreshToken);
			}
			else
				throw new AuthenticationException("Invalid password.");
		}
		public async Task<AuthResponse> LoginOrRegisterByGoogleIdAsync(string googleId, string email,
			string name)
		{
			User? user = await _userRepository.GetByGoogleIdAsync(googleId);
			if (user == null)
			{
				user = new User(email, null, name, email, null, null);
				user.SetGoogleId(googleId);
				await _userRepository.AddAsync(user);
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