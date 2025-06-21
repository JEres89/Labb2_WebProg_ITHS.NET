using MinimalAPI.DTOs.Requests.Customers;
using MinimalAPI.DTOs.Responses.Customers;
using MinimalAPI.DTOs.Responses.Orders;

namespace BlazorFrontend.Services;

public class CustomerService
{
	private const string customersUrl = "api/customers";

	private readonly HttpClient _httpClient;

	public CustomerService(HttpClient httpClient)
	{
		_httpClient = httpClient;
		//_httpClient.DefaultRequestHeaders.Add("CustomerServiceWasHere", "asdasd");
	}

	private async Task<T?> DoRequest<T>(Func<Task<HttpResponseMessage>> requestMethod, Action<string>? reportCallback = null)
	{
		//AddAuthHeader();
		return await Common.DoRequest<T>(requestMethod, reportCallback);
	}

	public async Task<CustomerCollectionResponse?> GetCustomersAsync(Action<string>? reportCallback = null) =>	
		await DoRequest<CustomerCollectionResponse>(() => _httpClient.GetAsync(customersUrl), reportCallback);

	public async Task<CustomerResponse?> GetCustomerAsync(int id, Action<string>? reportCallback = null) =>
		await DoRequest<CustomerResponse>(() => _httpClient.GetAsync( $"{customersUrl}/{id}"), reportCallback);

	public async Task<CustomerResponse?> CreateCustomerAsync(CustomerCreateRequest request, Action<string>? reportCallback = null) =>
		await DoRequest<CustomerResponse>(() => _httpClient.PostAsJsonAsync(customersUrl, request), reportCallback);

	public async Task<CustomerResponse?> UpdateCustomerAsync(int id, CustomerUpdateRequest request, Action<string>? reportCallback = null) => 
		await DoRequest<CustomerResponse>(() => _httpClient.PatchAsJsonAsync($"{customersUrl}/{id}", request), reportCallback);

	public async Task<bool> DeleteCustomerAsync(int id, Action<string>? reportCallback = null) =>
		await DoRequest<bool>(() => _httpClient.DeleteAsync($"{customersUrl}/{id}"), reportCallback);

	public async Task<OrderCollectionResponse?> GetCustomerOrdersAsync(int id, Action<string>? reportCallback = null) =>
		await DoRequest<OrderCollectionResponse>(() => _httpClient.GetAsync($"{customersUrl}/{id}/orders"), reportCallback);
}
