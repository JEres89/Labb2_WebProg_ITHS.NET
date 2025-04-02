namespace MinimalAPI.DataModels;

public class OrderProduct
{
	public int OrderId { get; set; }
	public required int ProductId { get; set; }
	public Product? Product { get; set; }
	public int Count { get; set; }
	public decimal Price { get; set; }
}
