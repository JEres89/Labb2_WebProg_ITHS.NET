using MinimalAPI.DataModels;

namespace MinimalAPI.DTOs.Responses.Orders;

public class OrderResponse
{
	public required int Id { get; set; }
	public required int CustomerId { get; set; }
	public required OrderStatus Status { get; set; }
	public required int[][] Products { get; set; }
}
