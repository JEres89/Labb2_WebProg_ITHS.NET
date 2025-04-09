namespace MinimalAPI.DataModels;

public class Order
{
	public int Id { get; set; }
	public required List<OrderProduct> Products { get; set; }
	public required int CustomerId { get; set; }
	public Customer? Customer { get; set; }
	public required OrderStatus Status { get; set; }
}
