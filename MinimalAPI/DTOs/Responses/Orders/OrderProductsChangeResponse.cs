namespace MinimalAPI.DTOs.Responses.Orders;

public record class OrderProductsChangeResponse
{
	public required int[][] Products { get; set; }
}
