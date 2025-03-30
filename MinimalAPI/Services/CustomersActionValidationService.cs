using MinimalAPI.Auth;
using MinimalAPI.DataModels;
using System.Diagnostics.CodeAnalysis;
using static MinimalAPI.Services.ICustomersActionValidationService;
using static MinimalAPI.Services.ValidationResultCode;

namespace MinimalAPI.Services;

public class CustomersActionValidationService : ICustomersActionValidationService
{
	private readonly IUnitOfWork _worker;
	public CustomersActionValidationService(IUnitOfWork worker)
	{
		_worker = worker;
	}

	public async Task<ValidationResult<Customer>> CreateCustomerAsync(WebUser? user, Customer customer)
	{
		if(user != null && user.Role != Role.Admin)
		{
			return new ValidationResult<Customer>
			{
				ResultCode = Unauthorized,
				ResultValue = null
			};
		}
		var repo = _worker.Customers;
		var newCustomer = await repo.CreateCustomerAsync(customer);
		await _worker.SaveChangesAsync();
		if(newCustomer == null)
		{
			return new ValidationResult<Customer>
			{
				ResultCode = Failed,
				ResultValue = newCustomer
			};
		}
		return new ValidationResult<Customer>
		{
			ResultCode = Success,
			ResultValue = newCustomer
		};
	}

	public async Task<ValidationResult<Customer>> GetCustomerAsync(WebUser? user, int id)
	{
		throw new NotImplementedException();
	}

	public async Task<ValidationResult<IEnumerable<Customer>>> GetCustomersAsync(WebUser? user)
	{
		throw new NotImplementedException();
	}

	public async Task<ValidationResult<Customer>> UpdateCustomerAsync(WebUser? user, int id, Dictionary<string, string> updates)
	{
		Customer customer;
		var repo = _worker.Customers;
		customer = await repo.GetCustomerAsync(id);
		if(customer == null)
		{
			return new ValidationResult<Customer> { 
				ResultCode = NotFound };
		}

		foreach(var prop in updates)
		{
			switch(prop.Key.ToLower())
			{
				case "firstname":
					customer.FirstName = prop.Value;
					break;
				case "lastname":
					customer.LastName = prop.Value;
					break;
				case "email":
					customer.Email = prop.Value;
					break;
				case "phone":
					customer.Phone = prop.Value;
					break;
				case "address":
					customer.Address = prop.Value;
					break;
				default:
					break;
			}
		}

		var updatedCustomer = await _worker.Customers.UpdateCustomerAsync(id, customer);
		await _worker.SaveChangesAsync();
		if(updatedCustomer == null)
		{
			return new ValidationResult<Customer>
			{
				ResultCode = Failed,
				ResultValue = updatedCustomer
			};
		}

		return new ValidationResult<Customer>
		{
			ResultCode = Success,
			ResultValue = updatedCustomer
		};
	}

	public async Task<ValidationResultCode> DeleteCustomerAsync(WebUser? user, int id)
	{
		if(user == null || (user.CustomerId != id && user.Role != Role.Admin))
		{
			return  Unauthorized;
		}

		var repo = _worker.Customers;
		var success = await repo.DeleteCustomerAsync(id);
		var count = await _worker.SaveChangesAsync();
		if(success && count > 0)
		{
			return Success;
		}
		return Failed;
	}

	public async Task<ValidationResult<IEnumerable<Order>>> GetOrdersAsync(WebUser? user, int id)
	{
		if(user == null || (user.CustomerId != id && user.Role != Role.Admin))
		{
			return new ValidationResult<IEnumerable<Order>>
			{
				ResultCode = Unauthorized,
				ResultValue = null
			};
		}

		var repo = _worker.Orders;
		var orders = await repo.FindOrdersAsync(o => o.CustomerId == id);
		if(orders == null)
		{
			return new ValidationResult<IEnumerable<Order>>
			{
				ResultCode = NotFound,
				ResultValue = null
			};
		}
		else if(orders.Count() == 0)
		{
			return new ValidationResult<IEnumerable<Order>>
			{
				ResultCode = Success,
				ResultValue = null
			};
		}
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
