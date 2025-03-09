using MinimalAPI.DataModels;

namespace MinimalAPI.DTOs.Requests;

/// <summary>
/// Get: Id in URL, not included.
/// Delete: Id in URL, not included.
/// 
/// Post: Everything but Id required in body.
/// Put: Id in URL, everything else in body.
/// Patch: Id in URL, modified fields in body.
/// </summary>
public class OrderRequest
{
	public int Id { get; set; } = -1;
	public (int productId, int count)[]? Products { get; set; } = null;
	public int CustomerId { get; set; } = -1;
	public OrderStatus Status { get; set; } = OrderStatus.undefined;
}
