using MinimalAPI.Auth;
using MinimalAPI.DataModels;
using static MinimalAPI.Services.ValidationResultCode;

namespace MinimalAPI.Services.Customers;

public class CustomersActionValidationService : ICustomersActionValidationService
{
	private readonly IUnitOfWork _worker;
	public CustomersActionValidationService(IUnitOfWork worker)
	{
		_worker = worker;
	}

	public async Task<ValidationResult<IEnumerable<Customer>>> GetCustomersAsync(WebUser? user)
	{
		if(user == null || user.Role != Role.Admin)
			return new ValidationResult<IEnumerable<Customer>> { ResultCode = Unauthorized };

		var repo = _worker.Customers;
		var customers = await repo.GetCustomersAsync();

		if(customers == null)
		{
			return new ValidationResult<IEnumerable<Customer>>
			{
				ResultCode = Failed,
				ErrorMessage = "Could not retreive Customers."
			};
		}
		else if(customers.Count() == 0)
		{
			return new ValidationResult<IEnumerable<Customer>> { ResultCode = Success };
		}
		else
		{
			return new ValidationResult<IEnumerable<Customer>>
			{
				ResultCode = Success,
				ResultValue = customers
			};
		}
	}
	public async Task<ValidationResult<Customer>> CreateCustomerAsync(WebUser? user, Customer customer)
	{
		if(user == null || user.Role != Role.Admin)
			return new ValidationResult<Customer> { ResultCode = Unauthorized };

		var repo = _worker.Customers;
		var newCustomer = await repo.CreateCustomerAsync(customer);
		var changes = await _worker.SaveChangesAsync();

		return new ValidationResult<Customer>
		{
			ResultCode = changes > 0 ? Success : Failed,
			ResultValue = newCustomer
		};
	}

	public async Task<ValidationResult<Customer>> GetCustomerAsync(WebUser? user, int id)
	{
		if(user == null || user.CustomerId != id && user.Role != Role.Admin)
			return new ValidationResult<Customer> { ResultCode = Unauthorized };

		var repo = _worker.Customers;
		var customer = await repo.GetCustomerAsync(id);

		return new ValidationResult<Customer>
		{
			ResultCode = customer == null ? NotFound : Success,
			ResultValue = customer
		};
	}


	public async Task<ValidationResult<Customer>> UpdateCustomerAsync(WebUser? user, int id, Dictionary<string, string> updates)
	{
		if(user == null || user.CustomerId != id && user.Role != Role.Admin)
			return new ValidationResult<Customer> { ResultCode = Unauthorized };

		var repo = _worker.Customers;
		var customer = await repo.GetCustomerAsync(id);

		if(customer == null)
			return new ValidationResult<Customer> { ResultCode = NotFound };

		Customer? updatedCustomer = null;

		try
		{
			updatedCustomer = await repo.UpdateCustomerAsync(id, updates);
		}
		catch(InvalidOperationException ex)
		{
			_worker.Dispose();
			if(ex.Message.Contains("could not be found"))
				return new ValidationResult<Customer>
				{
					ResultCode = Failed,
					ErrorMessage = "The property-name was not found. Properties are PascalCaseSensitive."
				};
		}

		var changes = await _worker.SaveChangesAsync();

		return new ValidationResult<Customer>
		{
			ResultCode = changes > 0 ? Success : Failed,
			ResultValue = changes > 0 ? updatedCustomer : null,
			ErrorMessage = changes > 0 ? null : "No changes were made"
		};
	}

	public async Task<ValidationResultCode> DeleteCustomerAsync(WebUser? user, int id)
	{
		if(user == null || user.CustomerId != id && user.Role != Role.Admin)
			return Unauthorized;

		var repo = _worker.Customers;
		var success = await repo.DeleteCustomerAsync(id);
		if(!success)
			return NotFound;

		var changes = await _worker.SaveChangesAsync();

		return changes > 0 ? Success : Failed;
	}

	public async Task<ValidationResult<IEnumerable<Order>>> GetOrdersAsync(WebUser? user, int id)
	{
		if(user == null || user.CustomerId != id && user.Role != Role.Admin)
			return new ValidationResult<IEnumerable<Order>> { ResultCode = Unauthorized };

		var repo = _worker.Orders;
		var orders = await repo.FindOrdersAsync(o => o.CustomerId == id);

		if(orders == null)
			return new ValidationResult<IEnumerable<Order>> { ResultCode = NotFound };

		else if(!orders.Any())
			return new ValidationResult<IEnumerable<Order>> { ResultCode = Success };

		else
		{
			return new ValidationResult<IEnumerable<Order>>
			{
				ResultCode = Success,
				ResultValue = orders
			};
		}
	}
}
