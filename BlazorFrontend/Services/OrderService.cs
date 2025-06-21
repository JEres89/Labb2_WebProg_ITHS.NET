using MinimalAPI.DTOs.Requests.Orders;
using MinimalAPI.DTOs.Responses.Orders;

namespace BlazorFrontend.Services;

public class OrderService
{
	private const string ordersUrl = "api/orders";

	private readonly HttpClient _httpClient;

	public OrderService(HttpClient httpClient)
	{
		_httpClient = httpClient;
		//_httpClient.DefaultRequestHeaders.Add("OrderServiceWasHere", "asdasd");
	}

	private async Task<T?> DoRequest<T>(Func<Task<HttpResponseMessage>> requestMethod, Action<string>? reportCallback = null)
	{
		//AddAuthHeader();
		return await Common.DoRequest<T>(requestMethod, reportCallback);
	}

	public async Task<OrderCollectionResponse?> GetOrdersAsync(Action<string>? reportCallback = null) =>	
		await DoRequest<OrderCollectionResponse>(() => _httpClient.GetAsync(ordersUrl), reportCallback);

	public async Task<OrderResponse?> GetOrderAsync(int id, Action<string>? reportCallback = null) =>
		await DoRequest<OrderResponse>(() => _httpClient.GetAsync( $"{ordersUrl}/{id}"), reportCallback);

	public async Task<OrderResponse?> CreateOrderAsync(OrderCreateRequest request, Action<string>? reportCallback = null) =>
		await DoRequest<OrderResponse>(() => _httpClient.PostAsJsonAsync(ordersUrl, request), reportCallback);

	public async Task<OrderResponse?> UpdateOrderAsync(int id, OrderUpdateRequest request, Action<string>? reportCallback = null) => 
		await DoRequest<OrderResponse>(() => _httpClient.PatchAsJsonAsync($"{ordersUrl}/{id}", request), reportCallback);

	public async Task<bool> DeleteOrderAsync(int id, Action<string>? reportCallback = null) =>
		await DoRequest<bool>(() => _httpClient.DeleteAsync($"{ordersUrl}/{id}"), reportCallback);

	public async Task<IEnumerable<OrderProductResponse>?> GetOrderProductsAsync(int id, Action<string>? reportCallback = null) =>
		await DoRequest<IEnumerable<OrderProductResponse>>(() => _httpClient.GetAsync($"{ordersUrl}/{id}/products"), reportCallback);

	public async Task<bool> UpdateOrderProductsAsync(int orderId, OrderProductsChangeRequest request, bool replace, Action<string>? reportCallback = null) => replace ? 
		await DoRequest<bool>(() => _httpClient.PutAsJsonAsync($"{ordersUrl}/{orderId}/products", request), reportCallback) :
		await DoRequest<bool>(() => _httpClient.PatchAsJsonAsync($"{ordersUrl}/{orderId}/products", request), reportCallback);
}
