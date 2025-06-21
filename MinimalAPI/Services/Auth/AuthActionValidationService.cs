using Microsoft.AspNetCore.Identity;
using Microsoft.Data.SqlClient;
using MinimalAPI.Auth;
using MinimalAPI.DataModels;
using MinimalAPI.DTOs;
using MinimalAPI.DTOs.Requests.Auth;
using MinimalAPI.DTOs.Responses.Auth;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using static System.Net.HttpStatusCode;

namespace MinimalAPI.Services.Auth;

public class AuthActionValidationService : IAuthActionValidationService
{
	private readonly IUnitOfWork _worker;
	public AuthActionValidationService(IUnitOfWork worker)
	{
		_worker = worker;
	}

	public async Task<ValidationResult<string>> RegisterAsync(RegisterRequest request)
	{
		if (string.IsNullOrEmpty(request.Email) || string.IsNullOrEmpty(request.Password))
			return new ValidationResult<string> {
				ResultCode = BadRequest,
				ErrorMessage = "Email and password are required." 
			};

		var canWork = await _worker.BeginWork<string>(true);
		if(canWork.ResultCode != Continue)
			return canWork;

		var repo = _worker.Auth;

		if((await repo.GetUserByEmailAsync(request.Email)) != null)
			return new ValidationResult<string> {
				ResultCode = Conflict,
				ErrorMessage = "There is already an account associated with this email address." 
			};

		var user = new WebUser {
			Email = request.Email,
			Role = Role.User
		};
		user.PasswordHash = new PasswordHasher<WebUser>().HashPassword(user, request.Password);

		try
		{
			if((await repo.AddUserAsync(user)) != null)
			{
				var changes = await _worker.SaveChangesAsync<string>();

				if(changes.ResultCode.IsSuccessCode())
				{
					changes.ResultCode = Created;
					changes.ResultValue = "Registration successful.";
					return changes;
				}
				else
					return changes;
			}
			else
				throw new Exception("Something went wrong, please try again later.");
		}
		catch(SqlException se)
		{
			if(ErrorHelper.DuplicateSqlErrors.Contains(se.Number)) // Unique constraint errors
				return await ErrorHelper.RollbackOnSqlServerDuplicateError<string>(_worker, se);
			else
				return await ErrorHelper.RollbackOnSqlServerError<string>(_worker, se, "creating the User");
		}
		catch(Exception e)
		{
			return await ErrorHelper.RollbackOnServerError<string>(_worker, e);
		}
	}
	public async Task<ValidationResult<LoginResponse>> LoginAsync(LoginRequest request)
	{
		if(string.IsNullOrEmpty(request.Email) || string.IsNullOrEmpty(request.Password))
			return new ValidationResult<LoginResponse> {
				ResultCode = BadRequest,
				ErrorMessage = "Email and password are required."
			};

		var user = await _worker.Auth.GetUserByEmailAsync(request.Email);

		if(user == null)
			return new ValidationResult<LoginResponse> {
				ResultCode = Unauthorized,
				ErrorMessage = "Invalid email or password."
			};

		var passwordHasher = new PasswordHasher<WebUser>();
		if(passwordHasher.VerifyHashedPassword(user, user.PasswordHash!, request.Password) == PasswordVerificationResult.Failed)
			return new ValidationResult<LoginResponse> {
				ResultCode = Unauthorized,
				ErrorMessage = "Invalid email or password."
			};

		if(user.CustomerId.HasValue && user.CustomerId.Value > 0)
			user.Customer = await _worker.Customers.GetCustomerAsync(user.CustomerId.Value);

		return new ValidationResult<LoginResponse> {
			ResultCode = OK,
			ResultValue = new LoginResponse {
				User = new WebUser { 
					Email = user.Email,
					Role = user.Role,
					CustomerId = user.CustomerId,
					Customer = user.Customer
				},
				Token = TokenGenerator.GenerateToken(SecureSecretVault.GetJwtSecret(), user)
			}
		};
	}

	public async Task<ValidationResult<WebUser>> GetUserAsync(string userEmail, ClaimsPrincipal user)
	{
		if(string.IsNullOrEmpty(userEmail))
			return await Task.FromResult(new ValidationResult<WebUser> {
				ResultCode = BadRequest,
				ErrorMessage = "Email is required."
			});

		if(!(user.IsInRole(Role.Admin.ToString()) || user.FindFirst(JwtRegisteredClaimNames.Email)?.Value == userEmail))
			return await Task.FromResult(new ValidationResult<WebUser> {
				ResultCode = Forbidden,
				ErrorMessage = "You do not have permission to access this user."
			});

		var webUser = await _worker.Auth.GetUserByEmailAsync(userEmail);
		if(webUser == null)
			return await Task.FromResult(new ValidationResult<WebUser> {
				ResultCode = NotFound,
				ErrorMessage = "User not found."
			});

		return await Task.FromResult(new ValidationResult<WebUser> {
			ResultCode = OK,
			ResultValue = webUser
		});
	}

	public async Task<ValidationResult<IEnumerable<WebUser>>> GetUsersAsync()
	{
		var allUsers = await _worker.Auth.GetUsersAsync();

		if(allUsers == null)
			return new ValidationResult<IEnumerable<WebUser>> {
				ResultCode = InternalServerError,
				ErrorMessage = "Could not retreive Users."
			};

		else if(allUsers.Count() == 0)
			return new ValidationResult<IEnumerable<WebUser>> {
				ResultCode = NoContent
			};

		else
			return new ValidationResult<IEnumerable<WebUser>> {
				ResultCode = OK,
				ResultValue = allUsers
			};
	}
	
	public async Task<ValidationResult<string>> DeleteUserAsync(string userEmail)
	{
		if(string.IsNullOrEmpty(userEmail))
			return new ValidationResult<string> {
				ResultCode = BadRequest,
				ErrorMessage = "Email is required."
			};

		var result = await _worker.Auth.DeleteUserAsync(userEmail);
		if(result)
			return new ValidationResult<string> { 
				ResultCode = NoContent 
			};

		else
			return new ValidationResult<string> {
				ResultCode = NotFound,
				ErrorMessage = "User not found."
			};
	}
}
