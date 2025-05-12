using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using System.IdentityModel.Tokens.Jwt;

using PCSetupHub.Core.DTOs;
using PCSetupHub.Core.Services;
using PCSetupHub.Core.Settings;

namespace PCSetupHub.Core.Middlewares
{
	public class TokenRefreshMiddleware(RequestDelegate next, IOptions<AuthSettings> options)
	{
		private readonly RequestDelegate _next = next;
		private readonly TokenSettings _accessTokenSettings = options.Value.AccessToken;
		private readonly TokenSettings _refreshTokenSettings = options.Value.RefreshToken;

		public async Task InvokeAsync(HttpContext httpContext, JwtService jwtService)
		{
			var accessToken = httpContext.Request.Cookies[_accessTokenSettings.CookieName];
			var refreshToken = httpContext.Request.Cookies[_refreshTokenSettings.CookieName];

			// If the access token is missing or has already expired
			if (!string.IsNullOrEmpty(refreshToken) &&
				(string.IsNullOrEmpty(accessToken) || IsTokenExpired(accessToken)))
			{
				var (isRefreshTokenValid, newTokens)
					= await jwtService.ValidateAndRefreshTokensAsync(accessToken!, refreshToken);

				if (isRefreshTokenValid)
				{
					UpdateTokenCookies(httpContext, newTokens);
					httpContext.Response.Cookies.Append("TokenRefreshed", "true",
						new CookieOptions
						{
							Expires = DateTime.UtcNow.AddSeconds(5),
							HttpOnly = false, // This is necessary so that JS can read it
							Path = "/"
						});

				}
			}

			await _next(httpContext);
		}

		private static bool IsTokenExpired(string token)
		{
			var handler = new JwtSecurityTokenHandler();
			var jwt = handler.ReadJwtToken(token);
			var expiration = jwt.ValidTo;

			return expiration < DateTime.UtcNow.AddSeconds(-5);
		}
		private void UpdateTokenCookies(HttpContext httpContext, AuthResponse newTokens)
		{
			bool rememberMe = GetRememberMeFromToken(newTokens.AccessToken!);
			DateTime? tokenExpires = !rememberMe ?
				null : DateTime.UtcNow.Add(_refreshTokenSettings.Lifetime);

			httpContext.Response.Cookies.Append(_refreshTokenSettings.CookieName,
				newTokens.RefreshToken!, new CookieOptions
				{ Expires = tokenExpires });

			httpContext.Response.Cookies.Append(_accessTokenSettings.CookieName,
				newTokens.AccessToken!, new CookieOptions
				{ Expires = tokenExpires });
		}
		private static bool GetRememberMeFromToken(string accessToken)
		{
			var handler = new JwtSecurityTokenHandler();
			var jwt = handler.ReadJwtToken(accessToken);

			var claim = jwt.Claims.FirstOrDefault(c => c.Type == "userRememberMe");
			return claim?.Value.ToLower() == "true";
		}
	}
}