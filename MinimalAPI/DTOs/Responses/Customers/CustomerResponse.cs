using MinimalAPI.DTOs.Responses.Orders;

namespace MinimalAPI.DTOs.Responses.Customers;

public class CustomerResponse
{
	public required int Id { get; set; }
	public required string FirstName { get; set; }
	public required string LastName { get; set; }
	public required string Email { get; set; }
	public string? Phone { get; set; }
	public string? Address { get; set; }
	public IEnumerable<OrderResponse>? Orders { get; set; }
	//public string? Orders { get; set; } // = "api/customers/{id}/orders"
}
