using MinimalAPI.Auth;
using MinimalAPI.DataModels;
using System.Diagnostics.CodeAnalysis;

namespace MinimalAPI.Services.Customers;

public interface ICustomersRepository : IDisposable
{
	Task<IEnumerable<Customer>> GetCustomersAsync();
	Task<Customer> CreateCustomerAsync(Customer customer);
	Task<Customer> GetCustomerAsync(int id);
	Task<Customer> UpdateCustomerAsync(int id, Customer customer);
	Task<bool> DeleteCustomerAsync(int id);
	Task<IEnumerable<Customer>> FindCustomersAsync(Predicate<Customer> customerMatch);
}

public interface ICustomersActionValidationService
{
	Task<ValidationResult<IEnumerable<Customer>>> GetCustomersAsync(WebUser? user);
	Task<ValidationResult<Customer>> CreateCustomerAsync(WebUser? user, Customer customer);
	Task<ValidationResult<Customer>> GetCustomerAsync(WebUser? user, int id);
	Task<ValidationResult<Customer>> UpdateCustomerAsync(WebUser? user, int id, Dictionary<string, string> updates);
	Task<ValidationResultCode> DeleteCustomerAsync(WebUser? user, int id);
	Task<ValidationResult<IEnumerable<Order>>> GetOrdersAsync(WebUser? user, int id);
}
