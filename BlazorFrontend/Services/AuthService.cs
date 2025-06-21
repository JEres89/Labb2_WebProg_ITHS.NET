using MinimalAPI.Auth;
using MinimalAPI.DTOs.Requests.Auth;
using MinimalAPI.DTOs.Responses.Auth;
using System.Net.Http.Headers;

namespace BlazorFrontend.Services;

public class AuthService
{
	private const string authUrl = "api/auth";
	private readonly HttpClient _httpClient;
	private string _jwtToken = string.Empty;

	public AuthService(HttpClient httpClient)
	{
		_httpClient = httpClient;
	}

	public WebUser? CurrentUser { get; private set; }

	public event Action? UserChanged;

	public bool IsAuthenticated => !string.IsNullOrEmpty(_jwtToken);

	internal void AddAuthHeader()
	{
		if(!string.IsNullOrEmpty(_jwtToken) && _httpClient.DefaultRequestHeaders.Authorization == null)
		{
			_httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _jwtToken);
		}
	}

	private async Task<T?> DoRequest<T>(Func<Task<HttpResponseMessage>> requestMethod, Action<string>? reportCallback = null)
	{
		//AddAuthHeader();
		return await Common.DoRequest<T>(requestMethod, reportCallback);
	}

	public async Task<string?> RegisterAsync(RegisterRequest request, Action<string>? reportCallback = null) =>
		await DoRequest<string>(() => _httpClient.PostAsJsonAsync($"{authUrl}/register", request), reportCallback);

	public async Task<bool> LoginAsync(LoginRequest request, Action<string>? reportCallback = null)
	{
		var response = await DoRequest<LoginResponse>(() => _httpClient.PostAsJsonAsync($"{authUrl}/login", request), reportCallback);

		if(response != null && !string.IsNullOrEmpty(response.Token))
		{
			_jwtToken = response.Token;
			_httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _jwtToken);
			CurrentUser = response.User;
			UserChanged?.Invoke();
			return true;
		}
		return false;
	}

	//public async Task LogoutAsync(JSRuntime jsRuntime, Action<string>? reportCallback = null)
	//{

	//	//await DoRequest<object>(() => _httpClient.PostAsync($"{authUrl}/logout", null), reportCallback);
	//	//_jwtToken = string.Empty;
	//	//_httpClient.DefaultRequestHeaders.Authorization = null;
	//}

	public async Task<WebUser?> GetUserAsync(string userEmail, Action<string>? reportCallback = null) =>
		await DoRequest<WebUser>(() => _httpClient.GetAsync($"{authUrl}/users/{userEmail}"), reportCallback);

	public async Task<UserCollectionResponse?> GetUsersAsync(Action<string>? reportCallback = null) =>
		await DoRequest<UserCollectionResponse>(() => _httpClient.GetAsync($"{authUrl}/users"), reportCallback);

	public async Task<bool> DeleteUserAsync(string userEmail, Action<string>? reportCallback = null) =>
		await DoRequest<bool>(() => _httpClient.DeleteAsync($"{authUrl}/users/{userEmail}"), reportCallback);
}
