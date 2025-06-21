using MinimalAPI.DTOs.Requests.Products;
using MinimalAPI.DTOs.Responses.Products;

namespace BlazorFrontend.Services;

public class ProductService
{
	private const string productsUrl = "api/products";

	private readonly HttpClient _httpClient;

	public ProductService(HttpClient httpClient)
	{
		_httpClient = httpClient;
		//_httpClient.DefaultRequestHeaders.Add("ProductServiceWasHere", "qweqwe");
	}

	private async Task<T?> DoRequest<T>(Func<Task<HttpResponseMessage>> requestMethod, Action<string>? reportCallback = null)
	{
		//AddAuthHeader();
		return await Common.DoRequest<T>(requestMethod, reportCallback);
	}

	public async Task<ProductCollectionResponse?> GetProductsAsync(Action<string>? reportCallback = null) =>	
		await DoRequest<ProductCollectionResponse>(() => _httpClient.GetAsync(productsUrl), reportCallback);

	public async Task<ProductResponse?> GetProductAsync(int id, Action<string>? reportCallback = null) =>
		await DoRequest<ProductResponse>(() => _httpClient.GetAsync( $"{productsUrl}/{id}"), reportCallback);

	public async Task<ProductResponse?> CreateProductAsync(ProductCreateRequest request, Action<string>? reportCallback = null) =>
		await DoRequest<ProductResponse>(() => _httpClient.PostAsJsonAsync(productsUrl, request), reportCallback);

	public async Task<ProductResponse?> UpdateProductAsync(int id, ProductUpdateRequest request, Action<string>? reportCallback = null) => 
		await DoRequest<ProductResponse>(() => _httpClient.PatchAsJsonAsync($"{productsUrl}/{id}", request), reportCallback);

	public async Task<bool> DeleteProductAsync(int id, Action<string>? reportCallback = null) =>
		await DoRequest<bool>(() => _httpClient.DeleteAsync($"{productsUrl}/{id}"), reportCallback);

	public async Task<IEnumerable<ProductOrderResponse>?> GetProductOrdersAsync(int id, Action<string>? reportCallback = null) =>
		await DoRequest<IEnumerable<ProductOrderResponse>>(() => _httpClient.GetAsync($"{productsUrl}/{id}/orders"), reportCallback);
}
