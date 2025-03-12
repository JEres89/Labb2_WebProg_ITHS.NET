using MinimalAPI.DataModels;

namespace MinimalAPI.DTOs.Responses;

public class ProductResponse
{
	public required string Name { get; set; }
	public required string Description { get; set; }
	public required string Category { get; set; }
	public required decimal Price { get; set; }
	public required ProductStatus Status { get; set; }
	public required int Stock { get; set; }
	/// <summary>
	/// Require administator role to view this property
	/// </summary>
	public List<int>? ActiveOrders { get; set; } = null;
}
