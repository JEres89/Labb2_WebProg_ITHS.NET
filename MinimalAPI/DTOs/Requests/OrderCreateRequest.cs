using MinimalAPI.DataModels;

namespace MinimalAPI.DTOs.Requests;

public class OrderCreateRequest
{
	public required int CustomerId { get; set; } = -1;
	/// <summary>
	/// Order is created when the first item is added to cart.
	/// </summary>
	public required KeyValuePair<int, int> Products { get; set; }
}
