namespace MinimalAPI.DTOs.Responses.Orders;

public class OrdersResponse
{
	public required IEnumerable<OrderResponse> Orders { get; set; }
}
