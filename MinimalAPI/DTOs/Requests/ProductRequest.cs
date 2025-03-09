namespace MinimalAPI.DTOs.Requests;

/// <summary>
/// Get: Id in URL, not included.
/// Delete: Id in URL, not included.
/// 
/// Post: Everything but Id required in body.
/// Put: Id in URL, everything else in body.
/// Patch: Id in URL, modified fields in body.
/// </summary>
public class ProductRequest
{
	//public  MyProperty { get; set; }
	public int Id { get; set; } = -1;
	public string? Name { get; set; } = null;
	public string? Description { get; set; } = null;
	public decimal Price { get; set; } = 0.0m;
	public int Stock { get; set; } = 0;
	//public string? Image { get; set; } = null;
}
