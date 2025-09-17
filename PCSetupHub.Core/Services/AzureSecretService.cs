using Azure.Identity;
using Azure.Security.KeyVault.Secrets;
using Microsoft.Extensions.Configuration;

using PCSetupHub.Core.Interfaces;

namespace PCSetupHub.Core.Services
{
	public class AzureSecretService : ISecretService
	{
		private static SecretClient? _client;

		public async Task<string> GetSecretAsync(IConfiguration configuration, string name)
		{
			if (string.IsNullOrWhiteSpace(name))
				throw new ArgumentException("Secret name must be provided", nameof(name));

			var client = GetClient(configuration);
			return (await client.GetSecretAsync(name)).Value.Value;
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