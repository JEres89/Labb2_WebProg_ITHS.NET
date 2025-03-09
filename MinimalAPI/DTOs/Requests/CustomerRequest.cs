namespace MinimalAPI.DTOs.Requests;

/// <summary>
/// Get: Id in URL, not included.
/// Delete: Id in URL, not included.
/// 
/// Post: Everything but Id required in body.
/// Put: Id in URL, everything else in body.
/// Patch: Id in URL, modified fields in body.
/// </summary>
public class CustomerRequest
{
	public int Id { get; set; } = -1;
	public string? Name { get; set; } = null;
	public string? Email { get; set; } = null;
	public string? Phone { get; set; } = null;
	public string? Address { get; set; } = null;
	//public List<int>? Orders { get; set; } = null;
}
