using MinimalAPI.Auth;
using MinimalAPI.DataModels;
using MinimalAPI.DTOs.Requests.Orders;

namespace MinimalAPI.Services;

public interface IOrdersRepository
{
	Task<IEnumerable<Order>> GetOrdersAsync();
	Task<Order> CreateOrderAsync(Order order);
	Task<Order> GetOrderAsync(int id);
	Task<Order> UpdateOrderAsync(int id, Order order);
	Task<bool> DeleteOrderAsync(int id);
	Task<IEnumerable<Order>> FindOrdersAsync(Predicate<Order> orderMatch);
	Task<IEnumerable<int[]>> EditProductsAsync(WebUser? user, int id, OrderProductsChangeRequest changeProducts);
}

public interface IOrdersActionValidationService
{
	Task<ValidationResult<IEnumerable<Order>>> GetOrdersAsync(WebUser? user);
	Task<ValidationResult<Order>> CreateOrderAsync(WebUser? user, Order order);
	Task<ValidationResult<Order>> GetOrderAsync(WebUser? user, int id);
	Task<ValidationResult<Order>> UpdateOrderAsync(WebUser? user, int id, OrderStatus status);
	Task<ValidationResultCode> DeleteOrderAsync(WebUser? user, int id);
	Task<ValidationResult<IEnumerable<Product>>> GetProductsAsync(WebUser? user, int id);
	Task<ValidationResult<IEnumerable<int[]>>> AddProductsAsync(WebUser? user, int id, IEnumerable<int[]> addProducts);
	Task<ValidationResult<IEnumerable<int[]>>> ReplaceProductsAsync(WebUser? user, int id, IEnumerable<int[]> newProducts);
	Task<ValidationResult<IEnumerable<int[]>>> EditProductsAsync(WebUser? user, int id, IEnumerable<int[]> changeProducts);
	Task<ValidationResult<IEnumerable<int[]>>> RemoveProductAsync(WebUser? user, int id, int pid);
}
