using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace MinimalAPI.Auth;

public class TokenGenerator
{
	public static string GenerateToken(string secret, WebUser user, int expirationMinutes = 60)
	{
		var tokenHandler = new JwtSecurityTokenHandler();
		var key = new System.Text.UTF8Encoding().GetBytes(secret);

		var claims = new List<Claim>
		{
			new Claim(JwtRegisteredClaimNames.Sub, user.Email),
			new Claim(JwtRegisteredClaimNames.Email, user.Email),
			new Claim(ClaimTypes.Role, user.Role.ToString())
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

		var token = tokenHandler.CreateToken(tokenDescriptor);
		return tokenHandler.WriteToken(token);
	}
}
