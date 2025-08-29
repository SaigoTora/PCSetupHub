using Microsoft.Extensions.Caching.Distributed;
using System.Text.Json;

using PCSetupHub.Core.Interfaces;

namespace PCSetupHub.Core.Services
{
	public class CacheService : ICacheService
	{
		private readonly IDistributedCache _cache;

		public CacheService(IDistributedCache cache)
		{
			_cache = cache;
		}

		public async Task SetAsync<T>(string key, T value, TimeSpan? expiration = null)
		{
			DistributedCacheEntryOptions options = new();

			if (expiration.HasValue)
				options.AbsoluteExpirationRelativeToNow = expiration;

			string json = JsonSerializer.Serialize(value);
			await _cache.SetStringAsync(key, json, options);
		}
		public async Task<T?> GetAsync<T>(string key)
		{
			string? json = await _cache.GetStringAsync(key);
			if (string.IsNullOrEmpty(json))
				return default;

			return JsonSerializer.Deserialize<T>(json);
		}
		public async Task RemoveAsync(string key) => await _cache.RemoveAsync(key);
	}
}