using Amazon.S3;
using Amazon.S3.Model;
using Amazon.S3.Transfer;
using Amazon.S3.Util;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;

using PCSetupHub.Core.Interfaces;
using PCSetupHub.Core.Settings;

namespace PCSetupHub.Core.Services
{
	public class ImageStorageService : IImageStorageService
	{
		private readonly IAmazonS3 _s3Client;
		private readonly string _bucketName;
		private static readonly string[] _allowedExtensions = [
			".jpg", ".jpeg", ".png", ".webp", ".gif",
			".bmp", ".ico", ".svg", ".tiff", ".tif",
			".heic", ".heif", ".jfif"
		];

		private static readonly Dictionary<ImageCategory, string> _categoryPaths = new()
		{
			[ImageCategory.Avatar] = "avatars/"
		};
		private bool? _isBucketExists = null;

		public ImageStorageService(IAmazonS3 s3Client, IOptions<AwsSettings> awsSettings)
		{
			_s3Client = s3Client;
			_bucketName = awsSettings.Value.BucketName;
		}

		public async Task<string> UploadImageAsync(IFormFile file, ImageCategory category)
		{
			if (file == null)
				throw new ArgumentNullException(nameof(file), "File cannot be null.");
			await EnsureBucketExistsAsync();

			string extension = Path.GetExtension(file.FileName).ToLowerInvariant();
			EnsureSupportedExtension(extension);

			string safeFileName = Path.GetFileNameWithoutExtension(file.FileName);
			if (!_categoryPaths.TryGetValue(category, out string? categoryPath))
				throw new ArgumentException($"Unknown image category: {category}",
					nameof(category));

			string key = $"{categoryPath}{safeFileName}_{Guid.NewGuid()}{extension}";

			TransferUtilityUploadRequest uploadRequest = new()
			{
				InputStream = file.OpenReadStream(),
				Key = key,
				BucketName = _bucketName,
				ContentType = file.ContentType
			};

			TransferUtility transferUtility = new(_s3Client);
			await transferUtility.UploadAsync(uploadRequest);

			return $"https://{_bucketName}.s3.us-east-1.amazonaws.com/{key}";
		}
		public async Task DeleteImageAsync(string imageUrl)
		{
			Uri uri = new(imageUrl);
			EnsureImageUrlIsValid(imageUrl, uri);

			await EnsureBucketExistsAsync();

			string key = uri.AbsolutePath.TrimStart('/');

			var deleteObjectRequest = new DeleteObjectRequest
			{
				BucketName = _bucketName,
				Key = key
			};

			await _s3Client.DeleteObjectAsync(deleteObjectRequest);
		}

		private async Task EnsureBucketExistsAsync()
		{
			if (_isBucketExists.HasValue)
				return;

			_isBucketExists = await AmazonS3Util.DoesS3BucketExistV2Async(_s3Client,
				_bucketName);

			if (!_isBucketExists.Value)
				throw new InvalidOperationException($"The S3 bucket '{_bucketName}' " +
					$"does not exist. Please make sure it is created and accessible.");
		}
		private static void EnsureSupportedExtension(string extension)
		{
			if (!_allowedExtensions.Contains(extension))
				throw new InvalidOperationException($"The file format '{extension}' " +
					$"is not supported. Only image files are allowed.");
		}
		private void EnsureImageUrlIsValid(string imageUrl, Uri imageUri)
		{
			if (string.IsNullOrWhiteSpace(imageUrl))
				throw new ArgumentException("Image URL cannot be null or empty.",
					nameof(imageUrl));

			string bucketHost = $"{_bucketName}.s3.us-east-1.amazonaws.com";

			if (!imageUri.Host.Equals(bucketHost, StringComparison.OrdinalIgnoreCase))
				throw new InvalidOperationException("The image URL does not belong " +
					"to the configured bucket.");
		}
	}
}