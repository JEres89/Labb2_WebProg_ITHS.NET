namespace MinimalAPI.DTOs.Requests.Auth;

public class RegisterRequest
{
	public required string Email { get; set; }
	public required string Password { get; set; }
}
