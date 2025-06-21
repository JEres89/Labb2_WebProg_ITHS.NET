using MinimalAPI.DataModels;
using System.Text.Json.Serialization;

namespace MinimalAPI.DTOs.Responses.Orders;

public class OrderResponse
{
	public required int Id { get; set; }
	public required int CustomerId { get; set; }
	public required OrderStatus Status { get; set; }

	//[JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
	public required int[][]? Products { get; set; }
}
