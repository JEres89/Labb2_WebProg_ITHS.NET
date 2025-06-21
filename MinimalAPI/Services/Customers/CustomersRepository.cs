using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using MinimalAPI.DataModels;
using MinimalAPI.Services.Database;

namespace MinimalAPI.Services.Customers;

public class CustomersRepository : ICustomersRepository
{
	private readonly ApiContext _context;
	public CustomersRepository(ApiContext context)
	{
		_context = context;
	}

	public async Task<bool> VerifyCustomer(int id)
	{
		return await _context.Customers.AnyAsync(c => c.Id == id);
	}
	public async Task<IEnumerable<Customer>> GetCustomersAsync()
	{
		return await _context.Customers.Include("Orders").IgnoreAutoIncludes().ToListAsync();
	}

	public async Task<Customer> CreateCustomerAsync(Customer customer)
	{
		return (await _context.Customers.AddAsync(customer)).Entity;
		//return customer;
	}

	public async Task<Customer?> GetCustomerAsync(int id)
	{
		return await _context.Customers.Include("Orders").IgnoreAutoIncludes().FirstOrDefaultAsync(c => c.Id == id);//.FindAsync(id);
	}

	public async Task<Customer?> UpdateCustomerAsync(int id, Dictionary<string, string> updates)
	{
		//Customer customer = new() { Id = id, FirstName = "", LastName = "", Email = "" };
		//var entity = _context.Attach<Customer>(customer);

		var customer = await _context.Customers.FindAsync(id);
		if(customer == null)
		{
			return null;
		}
		var entity = _context.Entry(customer);

		foreach(var prop in updates)
		{
			entity.SetProperty(prop.Key, prop.Value);
		}

		return customer;
	}

	public async Task<bool> DeleteCustomerAsync(int id)
	{
		var customer = await GetCustomerAsync(id);
		if(customer == null)
		{
			return false;
		}

		_context.Customers.Remove(customer);
		//await _context.SaveChangesAsync();

		return true;
	}

	public async Task<IEnumerable<Customer>> FindCustomersAsync(Predicate<Customer> customerMatch)
	{
		return await _context.Customers.Where(c => customerMatch(c)).ToListAsync();
		//return await Task.FromResult(_context.Customers.AsEnumerable().Where(c => customerMatch(c)).ToList());
	}

	public void Dispose()
	{
		GC.SuppressFinalize(this);
	}
}
