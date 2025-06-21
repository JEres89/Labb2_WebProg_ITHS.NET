using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using MinimalAPI.Auth;
using MinimalAPI.DTOs;
using MinimalAPI.DTOs.Requests.Auth;
using MinimalAPI.Services;
using MinimalAPI.Services.Auth;
using System.Security.Claims;

namespace MinimalAPI.Endpoints;

// TODO: Expand response types to include more specific error handling and success messages
public static class AuthEndpoints
{
	public static async Task<IResult> Register([FromBody] RegisterRequest request, IAuthActionValidationService authService)
	{
		var result = await authService.RegisterAsync(request);
		return result.ResultCode.IsSuccessCode() ? TypedResults.Ok(result.ResultValue) : TypedResults.BadRequest(result.ErrorMessage);
	}

	public static async Task<IResult> Login([FromBody] LoginRequest request, IAuthActionValidationService validation)
	{
		var result = await validation.LoginAsync(request);
		return result.ResultCode.IsSuccessCode() ? TypedResults.Ok(result.ResultValue) : TypedResults.Unauthorized();
	}

	//public static async Task<IResult> Logout(IAuthActionValidationService validation, ClaimsPrincipal user)
	//{
	//	await Task.CompletedTask;
	//	return TypedResults.Ok(new { Message = "Logged out successfully." });
	//}

	public static async Task<IResult> GetUser(string userEmail, IAuthActionValidationService validation, ClaimsPrincipal user)
	{
		var result = await validation.GetUserAsync(userEmail, user);
		return result.ResultCode.IsSuccessCode() ? TypedResults.Ok(result.ResultValue) : TypedResults.BadRequest(result.ErrorMessage);
	}

	public static async Task<IResult> GetUsers(IAuthActionValidationService validation)
	{
		var result = await validation.GetUsersAsync();
		return result.ResultCode.IsSuccessCode() ? TypedResults.Ok(result.ResultValue!.ToUserCollectionResponse()) : TypedResults.BadRequest(result.ErrorMessage);
	}

	public static async Task<IResult> DeleteUser(string userEmail, IAuthActionValidationService validation, ClaimsPrincipal user)
	{
		var result = await validation.DeleteUserAsync(userEmail);
		return result.ResultCode.IsSuccessCode() ? TypedResults.Ok(result.ResultValue) : TypedResults.BadRequest(result.ErrorMessage);
	}
}
