using MinimalAPI.DataModels;

namespace MinimalAPI.Services;

public interface ICustomersRepository
{
	Task<IEnumerable<Customer>> GetCustomersAsync();
	Task<Customer> CreateCustomerAsync(Customer customer);
	Task<Customer> GetCustomerAsync(int id);
	Task<Customer> UpdateCustomerAsync(int id, Customer customer);
	Task<bool> DeleteCustomerAsync(int id);
}
