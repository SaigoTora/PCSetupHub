using Microsoft.Extensions.Configuration;

namespace PCSetupHub.Core.Interfaces
{
	public interface ISecretService
	{
		/// <summary>
		/// Asynchronously retrieves a secret value using the provided configuration 
		/// and secret name.
		/// </summary>
		/// <param name="configuration">The application configuration containing necessary settings 
		/// for secret retrieval.</param>
		/// <param name="name">The name of the secret to retrieve.</param>
		/// <returns>A task that represents the asynchronous operation and contains the requested 
		/// secret value.</returns>
		Task<string> GetSecretAsync(IConfiguration configuration, string name);
	}
}