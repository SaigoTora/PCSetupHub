using Microsoft.Extensions.Configuration;

using PCSetupHub.Core.Interfaces;

namespace PCSetupHub.Core.Services
{
	public class UserSecretService : ISecretService
	{
		public Task<string> GetSecretAsync(IConfiguration configuration, string name)
		{
			if (string.IsNullOrWhiteSpace(name))
				throw new ArgumentException("Secret name must be provided", nameof(name));

			string? secretValue = configuration[name];

			if (string.IsNullOrEmpty(secretValue))
				throw new KeyNotFoundException($"Secret with name '{name}' was not found in " +
					$"configuration.");

			return Task.FromResult(secretValue);
		}
	}
}