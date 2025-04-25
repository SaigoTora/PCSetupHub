using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

using PCSetupHub.Core.DTOs;
using PCSetupHub.Core.Settings;
using PCSetupHub.Data.Models.Users;
using PCSetupHub.Data.Repositories.Interfaces;

namespace PCSetupHub.Core.Services
{
	public class JwtService(IOptions<AuthSettings> options, IUserRepository userRepository)
	{
		private readonly IOptions<AuthSettings> _options = options;
		private readonly IUserRepository _userRepository = userRepository;

		public async Task<(bool IsRefreshTokenValid, AuthResponse newTokens)>
			ValidateAndRefreshTokensAsync(string accessToken, string refreshToken)
		{
			JwtSecurityTokenHandler tokenHandler = new();

			try
			{
				tokenHandler.ValidateToken(refreshToken,
					GetTokenValidationParameters(_options.Value.RefreshToken.SecretKey),
					out SecurityToken validatedToken);
				if (validatedToken.ValidTo < DateTime.UtcNow)
					throw new SecurityTokenException("Refresh token expired.");

				// If refresh token is valid, validate access token
				bool accessTokenValid = true;
				try
				{
					tokenHandler.ValidateToken(accessToken,
						GetTokenValidationParameters(_options.Value.AccessToken.SecretKey),
						out validatedToken);
					if (validatedToken.ValidTo < DateTime.UtcNow)
						throw new SecurityTokenException("Access token expired.");
				}
				catch
				{ accessTokenValid = false; }

				if (accessTokenValid)
					return (true, new AuthResponse(null, null));

				return (true, await RefreshTokens(tokenHandler, accessToken));
			}
			catch
			{ return (false, new AuthResponse(null, null)); }
		}
		private async Task<AuthResponse> RefreshTokens(JwtSecurityTokenHandler tokenHandler,
			string accessToken)
		{
			JwtSecurityToken jwtToken = tokenHandler.ReadJwtToken(accessToken);
			Claim? userIdClaim = jwtToken.Claims.FirstOrDefault(c => c.Type == "userId")
				?? throw new SecurityTokenException("User ID claim not found in access token.");
			int userId = int.Parse(userIdClaim.Value);

			string newAccessToken = GenerateAccessToken(await _userRepository.GetOneAsync(userId));
			string newRefreshToken = GenerateRefreshToken();
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

		public string GenerateAccessToken(User? user)
			=> GenerateToken(_options.Value.AccessToken, user);
		public string GenerateRefreshToken()
			=> GenerateToken(_options.Value.RefreshToken);
		private static string GenerateToken(TokenSettings tokenSettings, User? user = null)
		{
			List<Claim>? claims = null;
			if (user != null)
				claims =
				[
					new Claim("userId", user.Id.ToString()),
					new Claim("userLogin", user.Login),
					new Claim("userName", user!.Name)
				];

			JwtSecurityToken jwtToken = new(
				expires: DateTime.UtcNow.Add(tokenSettings.Lifetime),
				claims: claims,
				signingCredentials:
				new SigningCredentials(new SymmetricSecurityKey(
					Encoding.UTF8.GetBytes(tokenSettings.SecretKey)),
					SecurityAlgorithms.HmacSha256));

			return new JwtSecurityTokenHandler().WriteToken(jwtToken);
		}
	}
}