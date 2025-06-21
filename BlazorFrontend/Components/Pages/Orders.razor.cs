using MinimalAPI.DataModels;
using MinimalAPI.DTOs.Requests.Orders;
using MinimalAPI.DTOs.Responses.Orders;

namespace BlazorFrontend.Components.Pages;
public partial class Orders
{
	private string? resultMessage;
	private void ResultMessage(string message)
	{
		// Handle the result message as needed
		InvokeAsync(() =>
		{
			resultMessage = message;
		});
	}


	#region GetOrders
	private OrderCollectionResponse? ordersCollection;

	/// <summary>
	/// GetOrdersAsync
	/// </summary>
	/// <returns></returns>
	private async Task GetOrdersAsync()
	{
		resultMessage = null;

		ordersCollection = await OrderService.GetOrdersAsync(ResultMessage);
	}
	#endregion


	#region GetOrder
	private int? getOrderId;
	private bool getOrderInvalid => (getOrderId??0) <= 0;

	private OrderResponse? getOrderResult;

	/// <summary>
	/// GetOrderAsync
	/// </summary>
	private async Task GetOrderAsync()
	{
		resultMessage = null;

		getOrderResult = await OrderService.GetOrderAsync(getOrderId!.Value, ResultMessage);
	}
	#endregion


	#region CreateOrder
	private int? customerId;
	private int? productId;
	private int? productCount;


	private bool createOrderInvalid =>
		(customerId??0) <= 0 || 
		(productId??0) <= 0 ||
		(productCount??0) <= 0;

	private OrderResponse? createOrderResult;

	/// <summary>
	/// CreateOrderAsync
	/// </summary>
	private async Task CreateOrderAsync()
	{
		resultMessage = null;
		var request = new OrderCreateRequest {
			CustomerId = customerId!.Value,
			Products = [[productId!.Value, productCount!.Value]]
		};
		createOrderResult = await OrderService.CreateOrderAsync(request, ResultMessage);
	}
	#endregion


	#region UpdateOrder
	private int? updateOrderId;
	private OrderStatus? updateOrderStatus;

	private bool updateOrderInvalid =>
		(updateOrderId??0) <= 0 ||
		updateOrderStatus == null;

	private OrderResponse? updateOrderResult;

	/// <summary>
	/// UpdateOrderAsync
	/// </summary>
	/// <returns></returns>
	private async Task UpdateOrderStatusAsync()
	{
		resultMessage = null;
		updateOrderResult = await OrderService.UpdateOrderAsync(updateOrderId!.Value, new OrderUpdateRequest { Status = updateOrderStatus!.Value}, ResultMessage);
	}
	#endregion


	#region DeleteOrder
	private int? deleteOrderId;
	private bool deleteOrderInvalid => (deleteOrderId??0) <= 0;

	private string deleteOrderResult = string.Empty;

	/// <summary>
	/// DeleteOrderAsync
	/// </summary>
	/// <returns></returns>
	private async Task DeleteOrderAsync()
	{
		resultMessage = null;
		var result = await OrderService.DeleteOrderAsync(deleteOrderId!.Value, ResultMessage);
		deleteOrderResult = result ? "Order deleted." : "Delete failed or error.";
	}
	#endregion


	#region GetOrderProducts
	private int? getOrderProductsId;
	private bool getOrderProductsValid => (getOrderProductsId??0) <= 0;

	private IEnumerable<OrderProductResponse>? orderProductsResult;

	/// <summary>
	/// GetOrderProductsAsync
	/// </summary>
	/// <returns></returns>
	private async Task GetOrderProductsAsync()
	{
		resultMessage = null;
		orderProductsResult = (await OrderService.GetOrderProductsAsync(getOrderProductsId!.Value, ResultMessage));

		if(orderProductsResult == null)
		{
			if(resultMessage == string.Empty)
			{
				resultMessage = "Unknown error occurred.";
			}
		}
		else if(!orderProductsResult.Any())
		{
			resultMessage = "No product in that order was found.";
		}
	}
	#endregion


	#region UpdateOrderProducts
	private int? updateOrderProductsId;
	private List<int?[]> updateOrderProducts = new List<int?[]>();
	private bool updateOrderProductInvalid =>
		(updateOrderProductsId??0) <= 0 ||
		(updateOrderProducts.Count == 0) ||
		!updateOrderProducts.Any(i => i[0] != null && i[1] != null);

	private bool? updateOrderProductsResult;

	private void RemoveProductToUpdate(int index)
	{
		updateOrderProducts.RemoveAt(index);
	}

	private void AddProductToUpdate()
	{
		updateOrderProducts.Add(new int?[] { null, null });
	}

	/// <summary>
	/// 
	/// </summary>
	/// <returns></returns>
	private async Task SetOrderProductsAsync() => 
		await UpdateOrderProductsAsync(false);

	/// <summary>
	/// 
	/// </summary>
	/// <returns></returns>
	private async Task ReplaceOrderProductAsync() => 
		await UpdateOrderProductsAsync(true);

	private async Task UpdateOrderProductsAsync(bool replace)
	{
		resultMessage = null;
		updateOrderProductsResult = null;

		var request = new OrderProductsChangeRequest {
			Products = [..updateOrderProducts
				.Where(static i => i[0] != null && i[1] != null)
				.Select(static i => new int[] { i[0]!.Value, i[1]!.Value })]
		};

		updateOrderProductsResult = await OrderService.UpdateOrderProductsAsync(updateOrderProductsId!.Value, request, replace, ResultMessage);
	}

	#endregion
}