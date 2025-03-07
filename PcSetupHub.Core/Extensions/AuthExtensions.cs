using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using System.Text;

using PCSetupHub.Core.Services;
using PCSetupHub.Core.Settings;

namespace PCSetupHub.Core.Extensions
{
	public static class AuthExtensions
	{
		public static IServiceCollection AddAuth(this IServiceCollection serviceCollection,
			IConfiguration configuration)
		{
			serviceCollection.AddScoped<JwtService>();
			serviceCollection.Configure<AuthSettings>(
					configuration.GetSection(nameof(AuthSettings)));

			var authSettings = configuration.GetSection(nameof(AuthSettings)).Get<AuthSettings>();

			serviceCollection.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
				.AddJwtBearer(o =>
				{
					o.TokenValidationParameters = new TokenValidationParameters()
					{
						ValidateIssuer = false,
						ValidateAudience = false,
						ValidateLifetime = true,
						ValidateIssuerSigningKey = true,
						IssuerSigningKey = new SymmetricSecurityKey(
							Encoding.UTF8.GetBytes(authSettings!.SecretKey))
					};
				});

			return serviceCollection;
		}
	}
}