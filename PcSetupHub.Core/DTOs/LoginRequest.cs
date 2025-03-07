namespace PCSetupHub.Core.DTOs
{
	public class LoginRequest
	{
		public required string Login { get; init; }
		public required string Password { get; init; }

		public LoginRequest()
		{ }
		public LoginRequest(string login, string password)
		{
			Login = login;
			Password = password;
		}
	}
}