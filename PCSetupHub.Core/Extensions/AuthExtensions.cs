using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

using PCSetupHub.Core.DTOs;
using PCSetupHub.Core.Services;
using PCSetupHub.Core.Settings;

namespace PCSetupHub.Core.Extensions
{
	public static class AuthExtensions
	{
		public static IServiceCollection AddExternalAuthProviders(this IServiceCollection services,
			IConfiguration configuration)
		{
			services.AddAuthentication(options =>
			{
				options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
				options.DefaultChallengeScheme = GoogleDefaults.AuthenticationScheme;
			})
			.AddCookie(CookieAuthenticationDefaults.AuthenticationScheme)
			.AddGoogle(googleOptions =>
			{
				googleOptions.ClientId = configuration["Authentication:Google:ClientId"]!;
				googleOptions.ClientSecret = configuration["Authentication:Google:ClientSecret"]!;
				googleOptions.SignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
			});
			return services;
		}
		public static IServiceCollection AddAuth(this IServiceCollection serviceCollection,
			IConfiguration configuration)
		{
			serviceCollection.AddScoped<JwtService>();
			serviceCollection.Configure<AuthSettings>(
					configuration.GetSection(nameof(AuthSettings)));

			AuthSettings? authSettings = configuration.GetSection(nameof(AuthSettings))
				.Get<AuthSettings>();

			if (authSettings == null || authSettings.AccessToken == null
				|| authSettings.RefreshToken == null)
				throw new InvalidOperationException("AuthSettings are not configured properly. " +
					"Please check appsettings.json.");

			TokenSettings accessTokenSettings = authSettings.AccessToken;
			TokenSettings refreshTokenSettings = authSettings.RefreshToken;

			serviceCollection.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
				.AddJwtBearer(o => ConfigureTokenOptions(o, refreshTokenSettings,
					accessTokenSettings));

			serviceCollection.AddAuthorization();
			return serviceCollection;
		}

		private static void ConfigureTokenOptions(JwtBearerOptions options,
			TokenSettings refreshTokenSettings, TokenSettings accessTokenSettings)
		{
			options.TokenValidationParameters
				= JwtService.GetTokenValidationParameters(accessTokenSettings.SecretKey);

			options.Events = new JwtBearerEvents
			{
				OnTokenValidated = async context =>
				{
					JwtService jwtService = context.HttpContext.RequestServices
						.GetRequiredService<JwtService>();

					await HandleTokenValidatedAsync(context, refreshTokenSettings,
						accessTokenSettings, jwtService);
				},
				OnChallenge = async context => await HandleChallengeAsync(context,
					refreshTokenSettings, accessTokenSettings),
				OnMessageReceived = context =>
				{
					context.Token = context.Request.Cookies[accessTokenSettings.CookieName];
					return Task.CompletedTask;
				}
			};
		}

		private static async Task HandleTokenValidatedAsync(TokenValidatedContext context,
			TokenSettings refreshTokenSettings, TokenSettings accessTokenSettings,
			JwtService jwtService)
		{
			if (!AreTokenCookiesConfigured(refreshTokenSettings, accessTokenSettings))
			{
				context.Fail("Token cookie names are not configured.");
				return;
			}

			string? refreshToken = context.Request.Cookies[refreshTokenSettings.CookieName];
			string? accessToken = context.Request.Cookies[accessTokenSettings.CookieName];

			if (string.IsNullOrEmpty(refreshToken) || string.IsNullOrEmpty(accessToken))
			{
				context.Fail("Missing tokens.");
				return;
			}

			var (isValidRefreshToken, newTokens)
				= await jwtService.ValidateAndRefreshTokensAsync(accessToken, refreshToken);

			if (!isValidRefreshToken)
				context.Fail("Invalid refresh token.");
			else if (newTokens.AccessToken != null && newTokens.RefreshToken != null)
				UpdateTokenCookies(context, refreshTokenSettings, accessTokenSettings, newTokens);
		}
		private static bool AreTokenCookiesConfigured(TokenSettings refreshTokenSettings,
			TokenSettings accessTokenSettings)
		{
			return !string.IsNullOrWhiteSpace(refreshTokenSettings.CookieName) &&
				!string.IsNullOrWhiteSpace(accessTokenSettings.CookieName);
		}
		private static void UpdateTokenCookies(TokenValidatedContext context,
			TokenSettings refreshTokenSettings, TokenSettings accessTokenSettings,
			AuthResponse newTokens)
		{
			var rememberMeClaim = context.Principal?.FindFirst("userRememberMe")?.Value;
			bool rememberMe = rememberMeClaim?.ToLower() == "true";

			DateTime? tokenExpires = !rememberMe ?
				null : DateTime.UtcNow.Add(refreshTokenSettings.Lifetime);

			context.Response.Cookies.Append(refreshTokenSettings.CookieName,
				newTokens.RefreshToken!, new CookieOptions
				{ Expires = tokenExpires });

			context.Response.Cookies.Append(accessTokenSettings.CookieName,
				newTokens.AccessToken!, new CookieOptions
				{ Expires = tokenExpires });
		}

		private static async Task HandleChallengeAsync(JwtBearerChallengeContext context,
			TokenSettings refreshTokenSettings, TokenSettings accessTokenSettings)
		{
			context.Response.Cookies.Delete(refreshTokenSettings.CookieName);
			context.Response.Cookies.Delete(accessTokenSettings.CookieName);

			if (!context.Response.HasStarted)
			{
				context.Response.Redirect("/Auth/Login");
				context.HandleResponse();
			}

			await Task.CompletedTask;
		}
	}
}