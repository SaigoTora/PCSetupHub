using Microsoft.Extensions.Configuration;

using PCSetupHub.Core.Interfaces;

namespace PCSetupHub.Core.Services
{
	public class TokenKeyService : ITokenKeyService
	{
		private const string ACCESS_TOKEN_SECRET_NAME = "Auth--AccessToken--SecretKey";
		private const string REFRESH_TOKEN_SECRET_NAME = "Auth--RefreshToken--SecretKey";

		private readonly IConfiguration _configuration;
		private readonly ISecretService _secretService;
		private string? _accessTokenKey;
		private string? _refreshTokenKey;
		private readonly SemaphoreSlim _accessTokenLock = new(1, 1);
		private readonly SemaphoreSlim _refreshTokenLock = new(1, 1);

		public TokenKeyService(IConfiguration configuration, ISecretService secretService)
		{
			_configuration = configuration;
			_secretService = secretService;
		}

		public async Task<string> GetAccessTokenKeyAsync()
		{
			if (_accessTokenKey != null)
				return _accessTokenKey;

			await _accessTokenLock.WaitAsync();
			try
			{
				if (_accessTokenKey == null)
				{
					_accessTokenKey = await _secretService.GetSecretAsync(_configuration,
						ACCESS_TOKEN_SECRET_NAME);
				}
				return _accessTokenKey;
			}
			finally
			{ _accessTokenLock.Release(); }
		}
		public async Task<string> GetRefreshTokenKeyAsync()
		{
			if (_refreshTokenKey != null)
				return _refreshTokenKey;

			await _refreshTokenLock.WaitAsync();
			try
			{
				if (_refreshTokenKey == null)
				{
					_refreshTokenKey = await _secretService.GetSecretAsync(_configuration,
						REFRESH_TOKEN_SECRET_NAME);
				}
				return _refreshTokenKey;
			}
			finally
			{ _refreshTokenLock.Release(); }
		}
	}
}