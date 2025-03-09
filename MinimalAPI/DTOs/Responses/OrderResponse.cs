using MinimalAPI.DataModels;

namespace MinimalAPI.DTOs.Responses;

/// <summary>
/// Get: Id in URL, not included.
/// Delete: Id in URL, not included.
/// 
/// Post: Everything but Id required in body.
/// Put: Id in URL, everything else in body.
/// Patch: Id in URL, modified fields in body.
/// </summary>
public class OrderResponse
{
	public required int Id { get; set; }
	public required (int productId, int count)[] Products { get; set; }
	public required int CustomerId { get; set; }
	public required OrderStatus Status { get; set; }
}
