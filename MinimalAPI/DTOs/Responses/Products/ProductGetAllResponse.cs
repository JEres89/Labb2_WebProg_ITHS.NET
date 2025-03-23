namespace MinimalAPI.DTOs.Responses.Products;

public record class ProductsResponse
{
	public required IEnumerable<ProductResponse> Products { get; init; }
}
