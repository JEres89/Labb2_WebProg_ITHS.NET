using MinimalAPI.DataModels;

namespace MinimalAPI.DTOs.Requests.Products;

public class ProductReplaceRequest
{
	public required string Name { get; set; }
	public required string Description { get; set; }
	public required string Category { get; set; }
	public required decimal Price { get; set; }
	public required ProductStatus Status { get; set; }
	public required int Stock { get; set; }
}
