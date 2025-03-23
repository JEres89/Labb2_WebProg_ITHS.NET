namespace MinimalAPI.DTOs.Requests.Products;

public class ProductCreateRequest
{
	public required string Name { get; set; }
	public required string Description { get; set; }
	public required string Category { get; set; }
	public decimal Price { get; set; } = 0.0m;
	public int Stock { get; set; } = 0;
}
