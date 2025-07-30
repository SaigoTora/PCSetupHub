using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using System.Threading.RateLimiting;

namespace PCSetupHub.Core.Extensions
{
	public static class RateLimiterExtensions
	{
		/// <summary>
		/// Adds and configures rate limiting policies for the application.
		/// Each policy can limit requests based on criteria like client IP and time windows.
		/// Also sets up a response to redirect clients when their requests are rejected due to rate limits.
		/// </summary>
		/// <param name="services">The service collection to configure.</param>
		public static void ConfigureRateLimiter(this IServiceCollection services)
		{
			services.AddRateLimiter(options =>
			{
				options.AddPolicy("LimitPerUser", context =>
				RateLimitPartition.GetFixedWindowLimiter(
					partitionKey: context.Connection.RemoteIpAddress?.ToString() ?? "global",
					factory: _ => new FixedWindowRateLimiterOptions
					{
						PermitLimit = 10,
						Window = TimeSpan.FromMinutes(7)
					}));

				options.OnRejected = async (context, token) =>
				{
					string redirectUrl = "/Error/429";

					context.HttpContext.Response.Redirect(redirectUrl);
					await Task.CompletedTask;
				};
			});
		}
	}
}