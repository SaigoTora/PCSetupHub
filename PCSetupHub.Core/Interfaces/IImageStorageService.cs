using Microsoft.AspNetCore.Http;

using PCSetupHub.Core.Settings;

namespace PCSetupHub.Core.Interfaces
{
	public interface IImageStorageService
	{
		/// <summary>
		/// Uploads an image file to the storage under the specified category.
		/// </summary>
		/// <param name="file">The image file to upload.</param>
		/// <param name="category">The category under which the image will be stored.</param>
		/// <returns>A URL string of the uploaded image.</returns>
		Task<string> UploadImageAsync(IFormFile file, ImageCategory category);

		/// <summary>
		/// Deletes an image from the storage by its URL.
		/// </summary>
		/// <param name="fileName">The URL of the image to delete.</param>
		/// <returns>A task representing the asynchronous operation.</returns>
		Task DeleteImageAsync(string fileName);
	}
}