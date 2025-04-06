namespace MinimalAPI.DataModels;

public class OrderProduct
{
	public required int OrderId { get; set; }
	public Order? Order { get; set; }
	public required int ProductId { get; set; }
	public Product? Product { get; set; }
	public int Count { get; set; } = -1;
	public decimal Price { get; set; } = -1;
}
