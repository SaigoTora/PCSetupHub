namespace PCSetupHub.Core.Interfaces
{
	public interface ICacheService
	{
		/// <summary>
		/// Saves an object in the cache under the given key.
		/// If expiration is not set, the key will stay in cache until removed.
		/// </summary>
		/// <typeparam name="T">Type of the object to save</typeparam>
		/// <param name="key">Cache key</param>
		/// <param name="value">Object to save</param>
		/// <param name="expiration">Optional expiration time</param>
		public Task SetAsync<T>(string key, T value, TimeSpan? expiration = null);

		/// <summary>
		/// Gets an object from the cache by key.
		/// Returns null if the key is not found or expired.
		/// </summary>
		/// <typeparam name="T">Type of the object</typeparam>
		/// <param name="key">Cache key</param>
		/// <returns>Object of type T or null</returns>
		public Task<T?> GetAsync<T>(string key);

		/// <summary>
		/// Removes an object from the cache by key.
		/// </summary>
		/// <param name="key">Cache key</param>
		/// <returns>A task representing the asynchronous operation.</returns>
		public Task RemoveAsync(string key);
	}
}