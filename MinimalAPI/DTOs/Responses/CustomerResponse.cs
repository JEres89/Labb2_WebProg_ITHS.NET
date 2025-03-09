namespace MinimalAPI.DTOs.Responses;

/// <summary>
/// Get: Id in URL, not included.
/// Delete: Id in URL, not included.
/// 
/// Post: Everything but Id required in body.
/// Put: Id in URL, everything else in body.
/// Patch: Id in URL, modified fields in body.
/// </summary>
public class CustomerResponse
{
	public required int Id { get; set; }
	public required string Name { get; set; }
	public required string Email { get; set; }
	public string? Phone { get; set; }
	public string? Address { get; set; }
	public List<int>? Orders { get; set; }
}
