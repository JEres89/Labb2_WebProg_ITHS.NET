namespace MinimalAPI.DataModels;

public class Order
{
	public int Id { get; init; }
	public required List<OrderProduct> Products { get; set; }
	public required int CustomerId { get; init; }
	public Customer? Customer { get; set; }
	public required OrderStatus Status { get; set; }
}
