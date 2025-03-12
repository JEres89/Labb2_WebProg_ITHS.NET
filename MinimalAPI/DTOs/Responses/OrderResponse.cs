using MinimalAPI.DataModels;

namespace MinimalAPI.DTOs.Responses;

public class OrderResponse
{
	//public required int Id { get; set; }
	public required string Products { get; set; } // = /orders/{id}/products
	public required int CustomerId { get; set; } 
	public required OrderStatus Status { get; set; }
}
