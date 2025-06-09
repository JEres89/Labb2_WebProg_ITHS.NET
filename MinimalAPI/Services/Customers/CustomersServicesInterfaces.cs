using MinimalAPI.Auth;
using MinimalAPI.DataModels;
using System.Net;
using System.Security.Claims;

namespace MinimalAPI.Services.Customers;

public interface ICustomersRepository : IDisposable
{
	Task<bool> VerifyCustomer(int id);
	Task<IEnumerable<Customer>> GetCustomersAsync();
	Task<Customer> CreateCustomerAsync(Customer customer);
	Task<Customer?> GetCustomerAsync(int id);
	Task<Customer?> UpdateCustomerAsync(int id, Dictionary<string, string> updates);
	Task<bool> DeleteCustomerAsync(int id);
	Task<IEnumerable<Customer>> FindCustomersAsync(Predicate<Customer> customerMatch);
}

public interface ICustomersActionValidationService
{
	Task<ValidationResult<IEnumerable<Customer>>> GetCustomersAsync();
	Task<ValidationResult<Customer>> CreateCustomerAsync(ClaimsPrincipal user, Customer customer);
	Task<ValidationResult<Customer>> GetCustomerAsync(ClaimsPrincipal user, int id);
	Task<ValidationResult<Customer>> UpdateCustomerAsync(ClaimsPrincipal user, int id, Dictionary<string, string> updates);
	Task<ValidationResult<int>> DeleteCustomerAsync(ClaimsPrincipal user, int id);
	Task<ValidationResult<IEnumerable<Order>>> GetOrdersAsync(ClaimsPrincipal user, int id);
}
