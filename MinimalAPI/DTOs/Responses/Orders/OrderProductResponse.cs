using MinimalAPI.DTOs.Responses.Products;

namespace MinimalAPI.DTOs.Responses.Orders;

public class OrderProductResponse
{
	public required string Path { get; set; }
	public required int Count { get; set; }
	public required decimal Price { get; set; }
	public required ProductResponse Product { get; set; }
}
