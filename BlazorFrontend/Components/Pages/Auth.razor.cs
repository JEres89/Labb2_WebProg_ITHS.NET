using BlazorFrontend.Services;
using MinimalAPI.Auth;
using MinimalAPI.DataModels;
using MinimalAPI.DTOs.Requests.Auth;
using MinimalAPI.DTOs.Responses.Auth;

namespace BlazorFrontend.Components.Pages;
public partial class Auth
{
	private string? resultMessage;
	private void ResultMessage(string message)
	{
		// Handle the result message as needed
		InvokeAsync(() =>
		{
			resultMessage = message;
		});
	}

	#region Login

	private string? loginEmail;
	private string? loginPassword;
	private bool loginInvalid =>
		string.IsNullOrWhiteSpace(loginEmail) ||
		string.IsNullOrWhiteSpace(loginPassword);

	private string? loginResult;


	/// <summary>
	/// 
	/// </summary>
	private async Task LoginAsync()
	{
		resultMessage = null;
		var request = new LoginRequest
		{
			Email = loginEmail!,
			Password = loginPassword!
		};
		var result = await AuthService.LoginAsync(request, ResultMessage);
		if (result)
		{
			loginResult = "Login successful.";
			StateHasChanged();
		}
		else
		{
			loginResult = "Login failed.";
		}
	}
	#endregion

	#region GetUsers
	private UserCollectionResponse? usersCollection;

	/// <summary>
	/// GetUsersAsync
	/// </summary>
	/// <returns></returns>
	private async Task GetUsersAsync()
	{
		resultMessage = null;

		usersCollection = await AuthService.GetUsersAsync(ResultMessage);
	}
	#endregion


	#region GetUser
	private string? getUserEmail;
	private bool getUserInvalid => string.IsNullOrWhiteSpace(getUserEmail);

	private WebUser? userResult;

	/// <summary>
	/// GetUserAsync
	/// </summary>
	private async Task GetUserAsync()
	{
		resultMessage = null;

		userResult = await AuthService.GetUserAsync(getUserEmail!, ResultMessage);
	}
	#endregion


	#region Register
	private string? email;
	private string? password;

	private bool registerInvalid =>
		string.IsNullOrWhiteSpace(email) ||
		string.IsNullOrWhiteSpace(password);

	private string? registerResult;

	/// <summary>
	/// RegisterAsync
	/// </summary>
	private async Task RegisterAsync()
	{
		resultMessage = null;
		var request = new RegisterRequest {
			Email = email!,
			Password = password!
		};
		registerResult = await AuthService.RegisterAsync(request, ResultMessage);
	}
	#endregion


	#region DeleteUser
	private string? deleteUserEmail;
	private bool deleteUserValid => string.IsNullOrWhiteSpace(deleteUserEmail);

	private string deleteUserResult = string.Empty;

	/// <summary>
	/// DeleteUserAsync
	/// </summary>
	/// <returns></returns>
	private async Task DeleteUserAsync()
	{
		resultMessage = null;
		var result = await AuthService.DeleteUserAsync(deleteUserEmail!, ResultMessage);
		deleteUserResult = result ? "User deleted." : "Delete failed or error.";
	}
	#endregion

}