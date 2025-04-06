using Microsoft.EntityFrameworkCore;
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

	public async Task<IEnumerable<Customer>> GetCustomersAsync()
	{
		return await _context.Customers.Include("Orders").IgnoreAutoIncludes().ToListAsync();
	}

	public async Task<Customer> CreateCustomerAsync(Customer customer)
	{
		_context.Customers.Add(customer);
		return customer;
	}

	public async Task<Customer?> GetCustomerAsync(int id)
	{
		return await _context.Customers.Include("Orders").IgnoreAutoIncludes().FirstOrDefaultAsync(c => c.Id == id);//.FindAsync(id);
	}

	//public async Task<bool> PatchCustomerAsync(int id, Dictionary<string, string> updates)
	//{
	//	Func<SetPropertyCalls<Customer>, SetPropertyCalls<Customer>> BuildPropertySetter = propertySetter => {
	//		foreach(var prop in updates)
	//		{
	//			switch(prop.Key)
	//			{
	//				case "firstname":
	//					propertySetter.SetProperty(c => c.FirstName, prop.Value);
	//					break;
	//				case "lastname":
	//					propertySetter.SetProperty(c => c.LastName, prop.Value);
	//					break;
	//				case "email":
	//					propertySetter.SetProperty(c => c.Email, prop.Value);
	//					break;
	//				case "phone":
	//					propertySetter.SetProperty(c => c.Phone, prop.Value);
	//					break;
	//				case "address":
	//					propertySetter.SetProperty(c => c.Address, prop.Value);
	//					break;
	//				default:
	//					break;
	//			}
	//		}
	//		return propertySetter;
	//	};

	//	var changes = await _context.Customers.Where(c => c.Id == id).ExecuteUpdateAsync(c => BuildPropertySetter(c));

	//	return changes > 0;
	//}

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

		//TODO: Add checking for case-incorrect property names
		foreach(var prop in updates)
		{
			entity.Member(prop.Key).CurrentValue = prop.Value;
		}

		return customer;
		//var changes = await _context.SaveChangesAsync();

		//return changes > 0 ? customer : null;
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
