using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;

using PCSetupHub.Core.Services;

namespace PCSetupHub.Tests.Integration
{
	public record TestData(int Id, string Name, bool IsActive);
	public class CacheServiceTests
	{
		private readonly IDistributedCache _cache;

		public CacheServiceTests()
		{
			_cache = new MemoryDistributedCache(
				new OptionsWrapper<MemoryDistributedCacheOptions>(
					new MemoryDistributedCacheOptions()));
		}

		#region Tests

		[Fact]
		public async Task SetAndGetAsync_StringValue_ReturnsSameString()
		{
			// Arrange
			string key = "testKey";
			string value = "testValue";

			// Act
			CacheService service = new(_cache);
			await service.SetAsync(key, value);
			string? result = await service.GetAsync<string>(key);

			// Assert
			Assert.Equal(value, result);

			// Clean up
			await _cache.RemoveAsync(key);
		}

		[Fact]
		public async Task SetAndGetAsync_DateTimeValue_ReturnsSameDateTime()
		{
			// Arrange
			string key = "testKey";
			DateTime value = DateTime.UtcNow;

			// Act
			CacheService service = new(_cache);
			await service.SetAsync(key, value);
			DateTime? result = await service.GetAsync<DateTime>(key);

			// Assert
			Assert.Equal(value, result);

			// Clean up
			await _cache.RemoveAsync(key);
		}

		[Fact]
		public async Task SetAsync_ValueWithExpiration_ExpiresAfterTime()
		{
			// Arrange
			string key = "testKey";
			int value = 127_648;
			TimeSpan expiration = TimeSpan.FromMilliseconds(500);

			// Act
			CacheService service = new(_cache);
			await service.SetAsync(key, value, expiration);
			int firstResult = await service.GetAsync<int>(key);
			await Task.Delay(700); // Wait for expiration
			int secondResult = await service.GetAsync<int>(key);

			// Assert
			Assert.Equal(value, firstResult);
			Assert.Equal(default, secondResult);

			// Clean up
			await _cache.RemoveAsync(key);
		}

		[Fact]
		public async Task SetAndRemoveAsync_TestDataValue_RemovedFromCache()
		{
			// Arrange
			string key = "testKey";
			TestData value = new(5, "John", true);

			// Act
			CacheService service = new(_cache);
			await service.SetAsync(key, value);
			TestData? firstResult = await service.GetAsync<TestData>(key);
			await _cache.RemoveAsync(key);
			TestData? secondResult = await service.GetAsync<TestData>(key);

			// Assert
			Assert.Equal(value, firstResult);
			Assert.Equal(default, secondResult);
		}

		#endregion
	}
}