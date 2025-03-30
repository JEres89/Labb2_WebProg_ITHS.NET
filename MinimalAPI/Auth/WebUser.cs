namespace MinimalAPI.Auth;

public class WebUser
{
	public required string UserName { get; set; }

	public Role Role { get; set; }
	public int CustomerId { get; set; } = 0;
}

public enum Role
{
	none,
	Admin,
	User
}
