namespace MinimalAPI.DataModels;

public class Customer
{
	public int Id { get; set; }
	public required string FirstName { get; set; }
	public required string LastName { get; set; }
	public required string Email { get; set; }
	public string? Phone { get; set; }
	public string? Address { get; set; }
	public List<Order>? Orders { get; set; }
}
