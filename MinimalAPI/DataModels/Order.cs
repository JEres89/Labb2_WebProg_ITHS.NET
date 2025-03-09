namespace MinimalAPI.DataModels;

public class Order
{
	public int Id { get; set; }
	public required List<(int productId, int count)> Products { get; set; } = null!;
	public required Customer Customer { get; set; }
	public required OrderStatus Status { get; set; }
}
