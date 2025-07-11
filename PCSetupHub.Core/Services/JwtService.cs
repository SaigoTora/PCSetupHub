using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

using PCSetupHub.Core.DTOs;
using PCSetupHub.Core.Settings;
using PCSetupHub.Data.Models.Users;
using PCSetupHub.Data.Repositories.Interfaces.Users;
using PCSetupHub.Core.Interfaces;

namespace PCSetupHub.Core.Services
{
	public class JwtService
	{
		private readonly IOptions<AuthSettings> _options;
		private readonly IUserRepository _userRepository;
		private readonly ITokenKeyService _tokenKeyService;

		public JwtService(IOptions<AuthSettings> options, ITokenKeyService tokenKeyService,
			IUserRepository userRepository)
		{
			_options = options;
			_tokenKeyService = tokenKeyService;
			_userRepository = userRepository;
		}

		public async Task<(bool isRefreshTokenValid, AuthResponse newTokens)>
			ValidateAndRefreshTokensAsync(string accessToken, string refreshToken)
		{
			JwtSecurityTokenHandler tokenHandler = new();

			try
			{
				string refreshKey = await _tokenKeyService.GetRefreshTokenKeyAsync();
				tokenHandler.ValidateToken(refreshToken,
					GetTokenValidationParameters(refreshKey), out SecurityToken validatedToken);
				if (validatedToken.ValidTo < DateTime.UtcNow)
					throw new SecurityTokenException("Refresh token expired.");

				// If refresh token is valid, validate access token
				bool accessTokenValid = true;
				try
				{
					string accessKey = await _tokenKeyService.GetAccessTokenKeyAsync();
					tokenHandler.ValidateToken(accessToken,
						GetTokenValidationParameters(accessKey), out validatedToken);
					if (validatedToken.ValidTo < DateTime.UtcNow)
						throw new SecurityTokenException("Access token expired.");
				}
				catch
				{ accessTokenValid = false; }

				if (accessTokenValid)
					return (true, new AuthResponse(null, null));

				return (true, await RefreshTokensAsync(tokenHandler, accessToken));
			}
			catch
			{ return (false, new AuthResponse(null, null)); }
		}
		private async Task<AuthResponse> RefreshTokensAsync(JwtSecurityTokenHandler tokenHandler,
			string accessToken)
		{
			JwtSecurityToken jwtToken = tokenHandler.ReadJwtToken(accessToken);
			Claim? userIdClaim = jwtToken.Claims.FirstOrDefault(c => c.Type == "userId")
				?? throw new SecurityTokenException("User ID claim not found in access token.");
			int userId = int.Parse(userIdClaim.Value);
			bool userRememberMe = bool.Parse(jwtToken.Claims
				.FirstOrDefault(c => c.Type == "userRememberMe")?.Value ?? "false");

			User? user = await _userRepository.GetOneAsync(userId);
			string newAccessToken = await GenerateAccessTokenAsync(user, userRememberMe);
			string newRefreshToken = await GenerateRefreshTokenAsync();
			return new AuthResponse(newAccessToken, newRefreshToken);
		}
		public static TokenValidationParameters GetTokenValidationParameters(string key)
		{
			return new TokenValidationParameters
			{
				ValidateIssuer = false,
				ValidateAudience = false,
				ValidateLifetime = true,
				ValidateIssuerSigningKey = true,
				IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key))
			};
		}

		public async Task<string> GenerateAccessTokenAsync(User? user, bool userRememberMe)
		{
			string key = await _tokenKeyService.GetAccessTokenKeyAsync();
			return GenerateToken(_options.Value.AccessToken, key, user, userRememberMe);
		}
		public async Task<string> GenerateRefreshTokenAsync()
		{
			string key = await _tokenKeyService.GetRefreshTokenKeyAsync();
			return GenerateToken(_options.Value.RefreshToken, key);
		}
		private static string GenerateToken(TokenSettings tokenSettings, string key,
			User? user = null, bool userRememberMe = false)
		{
			List<Claim>? claims = null;
			if (user != null)
				claims =
				[
					new Claim("userId", user.Id.ToString()),
					new Claim("userLogin", user.Login),
					new Claim("userName", user!.Name),
					new Claim("userRememberMe", userRememberMe.ToString())
				];

			JwtSecurityToken jwtToken = new(
				expires: DateTime.UtcNow.Add(tokenSettings.Lifetime),
				claims: claims,
				signingCredentials:
				new SigningCredentials(new SymmetricSecurityKey(
					Encoding.UTF8.GetBytes(key)),
					SecurityAlgorithms.HmacSha256));

			return new JwtSecurityTokenHandler().WriteToken(jwtToken);
		}
	}
}