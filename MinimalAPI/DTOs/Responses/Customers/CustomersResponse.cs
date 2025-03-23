namespace MinimalAPI.DTOs.Responses.Customers;

public record class CustomersResponse
{
	public required IEnumerable<CustomerResponse> Customers { get; init; }
}
