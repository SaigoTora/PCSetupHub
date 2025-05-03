using System.Security.Claims;

namespace PCSetupHub.Core.Extensions
{
	public static class ClaimsPrincipalExtensions
	{
		public static int? GetId(this ClaimsPrincipal user)
		{
			int? userId = null;

			if (Int32.TryParse(user?.Claims.FirstOrDefault(c => c.Type == "userId")?.Value,
				out int parsedUserId))
				userId = parsedUserId;

			return userId;
		}
		public static string? GetLogin(this ClaimsPrincipal user)
			=> user?.Claims.FirstOrDefault(c => c.Type == "userLogin")?.Value;
		public static string? GetName(this ClaimsPrincipal user)
			=> user?.Claims.FirstOrDefault(c => c.Type == "userName")?.Value;
	}
}