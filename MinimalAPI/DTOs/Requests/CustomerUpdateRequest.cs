namespace MinimalAPI.DTOs.Requests;

public class CustomerUpdateRequest
{
	public required KeyValuePair<string, string>[] Updates { get; set; }
}
