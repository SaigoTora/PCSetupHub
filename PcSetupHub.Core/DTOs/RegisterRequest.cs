namespace PCSetupHub.Core.DTOs
{
	public class RegisterRequest(string login, string password, string name, string email)
	{
		public string Login { get; init; } = login;
		public string Password { get; init; } = password;
		public string Name { get; init; } = name;
		public string Email { get; init; } = email;
	}
}