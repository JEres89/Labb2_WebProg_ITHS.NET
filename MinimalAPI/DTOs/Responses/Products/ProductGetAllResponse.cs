namespace MinimalAPI.DTOs.Responses.Products;

public record class ProductGetAllResponse
{
	public required IEnumerable<ProductResponse> Products { get; init; }
}
