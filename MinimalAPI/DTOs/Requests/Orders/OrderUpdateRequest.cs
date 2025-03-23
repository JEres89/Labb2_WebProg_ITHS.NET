using MinimalAPI.DataModels;

namespace MinimalAPI.DTOs.Requests.Orders;

public class OrderUpdateRequest
{
	public required OrderStatus Status { get; set; }
}
