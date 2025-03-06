using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

using PCSetupHub.Models.Users;

namespace PCSetupHub.Services
{
	public class JwtService(IOptions<AuthSettings> options)
	{
		private readonly IOptions<AuthSettings> _options = options;

		public string GenerateToken(User user)
		{
			List<Claim> claims =
			[
				new Claim("userName", user.Name),
				new Claim("id", user.Id.ToString())
			];

			var jwtToken = new JwtSecurityToken(
				expires: DateTime.UtcNow.Add(_options.Value.Expires),
				claims: claims,
				signingCredentials:
				new SigningCredentials(new SymmetricSecurityKey(
					Encoding.UTF8.GetBytes(_options.Value.SecretKey)),
					SecurityAlgorithms.HmacSha256));

			return new JwtSecurityTokenHandler().WriteToken(jwtToken);
		}
	}
}