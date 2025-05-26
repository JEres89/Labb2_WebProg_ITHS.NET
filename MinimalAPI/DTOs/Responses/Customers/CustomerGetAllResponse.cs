namespace MinimalAPI.DTOs.Responses.Customers;

public record class CustomerGetAllResponse
{
	public required IEnumerable<CustomerResponse> Customers { get; init; }
}
