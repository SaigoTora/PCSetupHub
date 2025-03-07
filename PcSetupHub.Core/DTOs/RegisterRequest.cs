namespace PCSetupHub.Core.DTOs
{
	public class RegisterRequest
	{
		public required string Login { get; init; }
		public required string Password { get; init; }
		public required string Name { get; init; }
		public required string Email { get; init; }

		public RegisterRequest()
		{ }
		public RegisterRequest(string login, string password, string name, string email)
		{
			Login = login;
			Password = password;
			Name = name;
			Email = email;
		}
	}
}