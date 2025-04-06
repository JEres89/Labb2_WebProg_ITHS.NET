using MinimalAPI.Auth;
using MinimalAPI.DataModels;

namespace MinimalAPI.Services.Orders;

public interface IOrdersRepository : IDisposable
{
	Task<IEnumerable<Order>> GetOrdersAsync();
	Task<Order> CreateOrderAsync(Order order);
	/// <summary>
	/// Applies lazy loading on <see cref="OrderProduct.Product"/> in <see cref="Order.Products"/> if <paramref name="withProducts"/> is <see langword="false"/>, meaning they will be null if not loaded.
	/// </summary>
	Task<Order?> GetOrderAsync(int id, bool withProducts = false);
	Task<Order?> UpdateOrderStatusAsync(int id, OrderStatus status);
	Task<bool> DeleteOrderAsync(int id);
	Task<IEnumerable<Order>> FindOrdersAsync(Predicate<Order> orderMatch);
	//Task<bool> EditProductAsync(int id, int productId, int amount);
	Task<Order?> UpdateProductsAsync(int id, IEnumerable<int[]> productChanges);
	Task<Order?> SetProductsAsync(int id, IEnumerable<int[]> newProducts);
	/// <summary>
	/// Applies lazy loading on <see cref="OrderProduct.Product"/>, meaning it will be null if not loaded.
	/// </summary>
	/// <param name="orderMatch">Pass in for filtering results, if null returns all orders with the product.</param>
	/// <returns></returns>
	Task<IEnumerable<OrderProduct>> FindOrdersForProductAsync(Func<OrderProduct, bool>? orderMatch, int productId);
}

public interface IOrdersActionValidationService
{
	Task<ValidationResult<IEnumerable<Order>>> GetOrdersAsync(WebUser? user);
	Task<ValidationResult<Order>> CreateOrderAsync(WebUser? user, Order order);
	Task<ValidationResult<Order>> GetOrderAsync(WebUser? user, int id);
	Task<ValidationResult<Order>> UpdateOrderAsync(WebUser? user, int id, OrderStatus status);
	Task<ValidationResultCode> DeleteOrderAsync(WebUser? user, int id);
	Task<ValidationResult<Order>> GetProductsAsync(WebUser? user, int id);
	Task<ValidationResult<Order>> UpdateProductsAsync(WebUser? user, int id, IEnumerable<int[]> setProducts);
	Task<ValidationResult<Order>> SetProductsAsync(WebUser? user, int id, IEnumerable<int[]> newProducts);
}
