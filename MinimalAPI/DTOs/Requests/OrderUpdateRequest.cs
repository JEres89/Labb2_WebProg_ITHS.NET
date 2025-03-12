using MinimalAPI.DataModels;

namespace MinimalAPI.DTOs.Requests;

public class OrderUpdateRequest
{
	public required KeyValuePair<string, int>[] Updates { get; set; }
}
