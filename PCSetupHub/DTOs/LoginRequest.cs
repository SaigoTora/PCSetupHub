namespace PCSetupHub.DTOs
{
	public class LoginRequest(string login, string password)
	{
		public string Login { get; init; } = login;
		public string Password { get; init; } = password;
	}
}