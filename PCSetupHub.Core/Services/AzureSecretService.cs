using Azure.Identity;
using Azure.Security.KeyVault.Secrets;
using Microsoft.Extensions.Configuration;

namespace PCSetupHub.Core.Services
{
	public static class AzureSecretService
	{
		private static SecretClient? _client;

		/// <summary>
		/// Asynchronously retrieves a secret from Azure Key Vault using the provided configuration and secret name.
		/// </summary>
		/// <param name="configuration">The application configuration containing the Key Vault settings.</param>
		/// <param name="name">The name of the secret to retrieve.</param>
		/// <returns>A task that represents the asynchronous operation and contains the requested secret.</returns>
		/// <exception cref="InvalidOperationException">
		/// Thrown if the Key Vault name is not configured in the application settings.
		/// </exception>
		public static async Task<KeyVaultSecret> GetSecretAsync(IConfiguration configuration,
			string name)
		{
			var client = GetClient(configuration);
			return await client.GetSecretAsync(name);
		}

		private static SecretClient GetClient(IConfiguration configuration)
		{
			if (_client != null) return _client;

			string? keyVaultName = configuration["Azure:KeyVault:Name"];
			if (string.IsNullOrEmpty(keyVaultName))
				throw new InvalidOperationException("Azure Key Vault name is not configured. " +
					"Please check appsettings.json or environment variables.");

			_client = new SecretClient(
				new Uri($"https://{keyVaultName}.vault.azure.net/"),
				new DefaultAzureCredential()
			);

			return _client;
		}
	}
}