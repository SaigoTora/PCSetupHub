using System.Security.Claims;

namespace PCSetupHub.Core.Extensions
{
	public static class ClaimsPrincipalExtensions
	{
		/// <summary>
		/// Gets the user ID from the claims if it exists and can be parsed as an integer.
		/// </summary>
		/// <param name="user">The ClaimsPrincipal instance.</param>
		/// <returns>User ID as nullable integer, or null if not found or invalid.</returns>
		public static int? GetId(this ClaimsPrincipal user)
		{
			int? userId = null;

			if (Int32.TryParse(user?.Claims.FirstOrDefault(c => c.Type == "userId")?.Value,
				out int parsedUserId))
				userId = parsedUserId;

			return userId;
		}

		/// <summary>
		/// Gets the user login from the claims if it exists.
		/// </summary>
		/// <param name="user">The ClaimsPrincipal instance.</param>
		/// <returns>User login string or null if not found.</returns>
		public static string? GetLogin(this ClaimsPrincipal user)
			=> user?.Claims.FirstOrDefault(c => c.Type == "userLogin")?.Value;

		/// <summary>
		/// Gets the user name from the claims if it exists.
		/// </summary>
		/// <param name="user">The ClaimsPrincipal instance.</param>
		/// <returns>User name string or null if not found.</returns>
		public static string? GetName(this ClaimsPrincipal user)
			=> user?.Claims.FirstOrDefault(c => c.Type == "userName")?.Value;

		/// <summary>
		/// Gets the "remember me" flag from the claims.
		/// Returns false if claim is missing or invalid.
		/// </summary>
		/// <param name="user">The ClaimsPrincipal instance.</param>
		/// <returns>True if "remember me" is set; otherwise, false.</returns>
		public static bool GetRememberMe(this ClaimsPrincipal user)
		{
			return bool.Parse(user?.Claims
				.FirstOrDefault(c => c.Type == "userRememberMe")?.Value ?? "false");
		}
	}
}