using Microsoft.AspNetCore.Http;

using PCSetupHub.Core.Settings;

namespace PCSetupHub.Core.Interfaces
{
	public interface IImageStorageService
	{
		Task<string> UploadImageAsync(IFormFile file, ImageCategory category);
		Task DeleteImageAsync(string fileName);
	}
}