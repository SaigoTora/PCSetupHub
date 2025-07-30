using Azure.Security.KeyVault.Secrets;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

using PCSetupHub.Core.DTOs;
using PCSetupHub.Core.Interfaces;
using PCSetupHub.Core.Services;
using PCSetupHub.Core.Settings;

namespace PCSetupHub.Core.Extensions
{
	public static class AuthExtensions
	{
		/// <summary>
		/// Adds external authentication providers (e.g., Google) to the service collection.
		/// </summary>
		/// <param name="services">The service collection to configure.</param>
		/// <param name="configuration">The application configuration.</param>
		/// <returns>The updated service collection.</returns>
		public static async Task<IServiceCollection> AddExternalAuthProvidersAsync(
			this IServiceCollection services, IConfiguration configuration)
		{
			KeyVaultSecret secretId = await AzureSecretService.GetSecretAsync(configuration,
				"GoogleClientId");
			string clientId = secretId.Value;
			KeyVaultSecret secretSecret = await AzureSecretService.GetSecretAsync(configuration,
				"GoogleClientSecret");
			string clientSecret = secretSecret.Value;

			services.AddAuthentication(options =>
			{
				options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
				options.DefaultChallengeScheme = GoogleDefaults.AuthenticationScheme;
			})
			.AddCookie(CookieAuthenticationDefaults.AuthenticationScheme)
			.AddGoogle(googleOptions =>
			{
				googleOptions.ClientId = clientId;
				googleOptions.ClientSecret = clientSecret;
				googleOptions.SignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
			});
			return services;
		}

		/// <summary>
		/// Configures JWT authentication and authorization services, including token settings validation.
		/// </summary>
		/// <param name="serviceCollection">The service collection to configure.</param>
		/// <param name="configuration">The application configuration.</param>
		/// <returns>The updated service collection.</returns>
		/// <exception cref="InvalidOperationException">Thrown when authentication settings are not configured properly.</exception>
		public async static Task<IServiceCollection> AddAuthAsync(
			this IServiceCollection serviceCollection, IConfiguration configuration)
		{
			serviceCollection.AddSingleton<ITokenKeyService, TokenKeyService>();
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
			TokenKeyService tokenKeyService = new(configuration);
			string accessTokenKey = await tokenKeyService.GetAccessTokenKeyAsync();

			serviceCollection.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
				.AddJwtBearer(o => ConfigureTokenOptions(o, refreshTokenSettings,
					accessTokenSettings, accessTokenKey));

			serviceCollection.AddAuthorization();
			return serviceCollection;
		}

		private static void ConfigureTokenOptions(JwtBearerOptions options,
			TokenSettings refreshTokenSettings, TokenSettings accessTokenSettings,
			string accessTokenKey)
		{
			options.TokenValidationParameters
				= JwtService.GetTokenValidationParameters(accessTokenKey);

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