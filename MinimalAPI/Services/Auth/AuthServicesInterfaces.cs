using MinimalAPI.Auth;
using MinimalAPI.DataModels;
using MinimalAPI.DTOs.Requests.Auth;
using MinimalAPI.DTOs.Responses.Auth;
using System.Security.Claims;

namespace MinimalAPI.Services.Auth;

public interface IAuthRepository : IDisposable
{
	Task<IEnumerable<WebUser>> GetUsersAsync();
	Task<WebUser?> GetUserByEmailAsync(string email);
	Task<WebUser?> AddUserAsync(WebUser user);
	Task<bool> DeleteUserAsync(string email);
}

public interface IAuthActionValidationService
{
	Task<ValidationResult<string>> RegisterAsync(RegisterRequest request);
	Task<ValidationResult<LoginResponse>> LoginAsync(LoginRequest request);
	Task<ValidationResult<WebUser>> GetUserAsync(string userEmail, ClaimsPrincipal user);
	Task<ValidationResult<IEnumerable<WebUser>>> GetUsersAsync();
	Task<ValidationResult<string>> DeleteUserAsync(string userEmail);
}
