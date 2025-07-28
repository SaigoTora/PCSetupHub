using PCSetupHub.Data.Models.Users;

namespace PCSetupHub.Core.Interfaces
{
	public interface IUserAccessService
	{
		Task<bool> HasAccessAsync(string requesterLogin, string targetLogin,
			PrivacyLevelType level);
	}
}