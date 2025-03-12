using System.Text.Json.Nodes;

namespace MinimalAPI.DTOs.Requests;

public class ProductUpdateRequest
{
	public required KeyValuePair<string, object>[] Updates { get; set; }
}
