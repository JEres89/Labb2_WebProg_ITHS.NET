namespace MinimalAPI.DTOs.Requests.Customers;

public class CustomerCreateRequest
{
	public required string FirstName { get; set; }
	public required string LastName { get; set; }
	public required string Email { get; set; }
	public string? Phone { get; set; } = null;
	public string? Address { get; set; } = null;
}
