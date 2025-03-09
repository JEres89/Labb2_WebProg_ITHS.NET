namespace MinimalAPI.DataModels;

public class Product
{
	public int Id { get; set; }
	public required string Name { get; set; } = null!;
	public required string Description { get; set; } = null!;
	public required decimal Price { get; set; }
	public int Stock { get; set; }
	public List<int>? ActiveOrders { get; set; }
}