using Microsoft.EntityFrameworkCore;
using MinimalAPI.DataModels;
using System.Text.Json.Serialization;

namespace MinimalAPI.Auth;

[PrimaryKey(nameof(Email))]
public class WebUser
{
	public required string Email { get; set; }
	[JsonIgnore]
	public string? PasswordHash { get; set; }
	public Role Role { get; set; }
	public int? CustomerId { get; set; }
	public Customer? Customer { get; set; }
}

public enum Role
{
	none,
	Admin,
	User
}
