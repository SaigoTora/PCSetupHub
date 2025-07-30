namespace PCSetupHub.Core.Interfaces
{
	public interface ITokenKeyService
	{
		/// <summary>
		/// Retrieves the secret key for access tokens asynchronously.
		/// </summary>
		/// <returns>A task that returns the access token secret key as a string.</returns>
		Task<string> GetAccessTokenKeyAsync();

		/// <summary>
		/// Retrieves the secret key for refresh tokens asynchronously.
		/// </summary>
		/// <returns>A task that returns the refresh token secret key as a string.</returns>
		Task<string> GetRefreshTokenKeyAsync();
	}
}