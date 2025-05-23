using MinimalAPI.Auth;
using MinimalAPI.DataModels;
using System.Linq.Expressions;
using System.Net;

namespace MinimalAPI.Services.Orders;

public interface IOrdersRepository : IDisposable
{
	Task<IEnumerable<Order>> GetOrdersAsync();
	Task<Order> CreateOrderAsync(Order order, IEnumerable<int[]>? products);
	/// <summary>
	/// Applies lazy loading on <see cref="OrderProduct.Product"/> in <see cref="Order.Products"/> if <paramref name="withProducts"/> is <see langword="false"/>, meaning they will be null if not loaded.
	/// </summary>
	Task<Order?> GetOrderAsync(int orderId, bool withProducts = false);
	Task<Order?> UpdateOrderStatusAsync(int orderId, OrderStatus status);
	Task<bool> DeleteOrderAsync(int orderId);
	Task<IEnumerable<Order>> FindOrdersAsync(Expression<Func<Order, bool>> orderMatch);
	Task<Order?> UpdateProductsAsync(int orderId, IEnumerable<int[]> productChanges);
	Task<Order?> SetProductsAsync(int orderId, IEnumerable<int[]> newProducts);
	/// <summary>
	/// Applies lazy loading on <see cref="OrderProduct.Product"/>, meaning it will be null if not loaded.
	/// </summary>
	/// <param name="orderMatch">Pass in for filtering results, if null returns all orders with the product.</param>
	/// <returns></returns>
	Task<IEnumerable<OrderProduct>?> FindOrdersForProductAsync(Expression<Func<OrderProduct, bool>>? orderMatch, int productId);
}

public interface IOrdersActionValidationService
{
	Task<ValidationResult<IEnumerable<Order>>> GetOrdersAsync(WebUser? user);
	Task<ValidationResult<Order>> CreateOrderAsync(WebUser? user, Order order, int[][]? products);
	Task<ValidationResult<Order>> GetOrderAsync(WebUser? user, int orderId);
	Task<ValidationResult<Order>> UpdateOrderAsync(WebUser? user, int orderId, OrderStatus status);
	Task<ValidationResult<int>> DeleteOrderAsync(WebUser? user, int orderId);
	Task<ValidationResult<Order>> GetProductsAsync(WebUser? user, int orderId);
	Task<ValidationResult<Order>> UpdateProductsAsync(WebUser? user, int orderId, IEnumerable<int[]> setProducts, bool replace = false);
}
