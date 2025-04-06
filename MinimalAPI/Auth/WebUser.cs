using MinimalAPI.DataModels;

namespace MinimalAPI.Auth;

public class WebUser
{
	public required string UserName { get; set; }

	public Role Role { get; set; }
	public int CustomerId { get; set; } = -1;
	public Customer? Customer { get; set; }
}

public enum Role
{
	none,
	Admin,
	User
}
