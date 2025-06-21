using MinimalAPI.Auth;

namespace MinimalAPI.DTOs.Responses.Auth;

public class LoginResponse
{
    public WebUser? User { get; set; }
    public string? Token { get; set; }
}
