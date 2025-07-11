namespace PCSetupHub.Core.Interfaces
{
	public interface ITokenKeyService
	{
		Task<string> GetAccessTokenKeyAsync();
		Task<string> GetRefreshTokenKeyAsync();
	}
}