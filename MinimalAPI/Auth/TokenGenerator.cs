using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;

namespace MinimalAPI.Auth;

public class TokenGenerator
{
	public static string GenerateToken(string secret, WebUser user, int expirationMinutes = 60)
	{
		var tokenHandler = new JsonWebTokenHandler();

		var key = new System.Text.UTF8Encoding().GetBytes(secret);

		var claims = new List<Claim>
		{
			new Claim(JwtRegisteredClaimNames.Sub, user.Email),
			new Claim(JwtRegisteredClaimNames.Email, user.Email),
			new Claim("role", user.Role.ToString())
		};
		if(user.Role == Role.User)
		{
			claims.Add(new Claim("CustomerId", user.CustomerId?.ToString()??string.Empty, "int"));
		}
		
		var tokenDescriptor = new SecurityTokenDescriptor {
			Subject = new ClaimsIdentity(claims),
			Issuer = "JensEresund",
			Audience = "ApiConsumers",
			Expires = DateTime.UtcNow.AddMinutes(expirationMinutes),
			SigningCredentials = new SigningCredentials(
				new SymmetricSecurityKey(key),
				SecurityAlgorithms.HmacSha256Signature)
		};

		return tokenHandler.CreateToken(tokenDescriptor);
	}
}
