using MinimalAPI.Auth;
using MinimalAPI.DataModels;
using System.Diagnostics.CodeAnalysis;

namespace MinimalAPI.Services;

public interface IOrdersRepository
{
	Task<IEnumerable<Order>> GetOrdersAsync();
	Task<Order> CreateOrderAsync(Order order);
	Task<Order> GetOrderAsync(int id);
	Task<Order> UpdateOrderAsync(int id, Order order);
	Task<bool> DeleteOrderAsync(int id);
	Task<IEnumerable<Order>> FindOrdersAsync(Predicate<Order> orderMatch);
}

public interface IOrdersActionValidationService
{
	Task<ValidationResult<IEnumerable<Order>>> GetOrdersAsync(WebUser? user);
	Task<ValidationResult<Order>> GetOrderAsync(WebUser? user, int id);
	Task<ValidationResult<Order>> CreateOrderAsync(WebUser? user, Order order);
	Task<ValidationResult<Order>> UpdateOrderAsync(WebUser? user, int id, Dictionary<string, string> updates);
	Task<ValidationResultCode> DeleteOrderAsync(WebUser? user, int id);
	
}
