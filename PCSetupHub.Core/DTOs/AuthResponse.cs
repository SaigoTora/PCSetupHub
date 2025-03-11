﻿namespace PCSetupHub.Core.DTOs
{
	public class AuthResponse(string? accessToken, string? refreshToken)
	{
		public string? AccessToken { get; init; } = accessToken;
		public string? RefreshToken { get; init; } = refreshToken;
	}
}