namespace MinimalAPI.DataModels;

public class Customer
{
	public int Id { get; set; }
	public required string Name { get; set; }
	public required string Email { get; set; }
	public string? Phone { get; set; }
	public string? Address { get; set; }
	public List<int> Orders { get; set; }
}
