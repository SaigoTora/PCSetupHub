using Azure.Identity;
using Azure.Security.KeyVault.Secrets;
using Microsoft.Extensions.Configuration;

namespace PCSetupHub.Core.Services
{
	public static class AzureSecretService
	{
		private static SecretClient? _client;

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

		public static async Task<KeyVaultSecret> GetSecretAsync(IConfiguration configuration,
			string name)
		{
			var client = GetClient(configuration);
			return await client.GetSecretAsync(name);
		}
	}
}