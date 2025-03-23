using MinimalAPI.DTOs.Responses.Orders;

namespace MinimalAPI.DTOs.Responses.Products;

public class ProductOrderResponse
{
	public required string Path { get; set; }
	public required int Count { get; set; }
	public required decimal Price { get; set; }
	public required OrderResponse Order { get; set; }
}
