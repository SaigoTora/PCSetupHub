using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.Extensions.DependencyInjection;
using System.Threading.RateLimiting;

namespace PCSetupHub.Core.Extensions
{
	public static class RateLimiterExtensions
	{
		private static readonly string[] _staticFileExtensions = [
			".css", ".js", ".png", ".jpg", ".jpeg", ".gif", ".ico",
			".svg", ".woff", ".woff2", ".ttf", ".eot", ".map", ".json"
		];

		/// <summary>
		/// Configures rate limiting for the application, including a global limiter
		/// and multiple named policies for specific types of user actions.
		/// Redirects users to an error page when a rate limit is exceeded.
		/// </summary>
		/// <param name="services">The service collection to configure.</param>
		public static void ConfigureRateLimiter(this IServiceCollection services)
		{
			services.AddRateLimiter(options =>
			{
				options.GlobalLimiter = CreateGlobalLimiter(1000, TimeSpan.FromMinutes(10));

				AddFixedWindowPolicy(options, "AccountCritical", 20, TimeSpan.FromMinutes(10));
				AddFixedWindowPolicy(options, "Friendship", 50, TimeSpan.FromMinutes(10));
				AddFixedWindowPolicy(options, "Comments", 50, TimeSpan.FromMinutes(10));
				AddFixedWindowPolicy(options, "UploadAvatar", 5, TimeSpan.FromMinutes(10));
				AddFixedWindowPolicy(options, "PcConfiguration", 75, TimeSpan.FromMinutes(10));
				AddFixedWindowPolicy(options, "ApplySettings", 25, TimeSpan.FromMinutes(10));

				options.OnRejected = async (context, token) =>
				{
					string redirectUrl = "/Error/429";

					context.HttpContext.Response.Redirect(redirectUrl);
					await Task.CompletedTask;
				};
			});
		}

		private static PartitionedRateLimiter<HttpContext> CreateGlobalLimiter(int permitLimit,
			TimeSpan window)
		{
			return PartitionedRateLimiter.Create<HttpContext, string>(httpContext =>
			{
				string path = httpContext.Request.Path.Value ?? "";
				string ext = Path.GetExtension(path).ToLowerInvariant();

				// Exclude statics by extension or by path
				if (_staticFileExtensions.Contains(ext))
				{
					return RateLimitPartition.GetNoLimiter("static");
				}

				string key = httpContext.Connection.RemoteIpAddress?.ToString() ?? "global";

				return RateLimitPartition.GetFixedWindowLimiter(
					key,
					_ => new FixedWindowRateLimiterOptions
					{
						PermitLimit = permitLimit,
						Window = window
					});
			});
		}
		private static void AddFixedWindowPolicy(RateLimiterOptions options, string policyName,
			int permitLimit, TimeSpan window)
		{
			options.AddPolicy(policyName, context =>
				RateLimitPartition.GetFixedWindowLimiter(
					partitionKey: context.Connection.RemoteIpAddress?.ToString() ?? "global",
					factory: _ => new FixedWindowRateLimiterOptions
					{
						PermitLimit = permitLimit,
						Window = window
					}));
		}
	}
}