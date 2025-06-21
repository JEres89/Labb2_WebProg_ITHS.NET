using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore.Internal;
using MinimalAPI.Auth;
using MinimalAPI.DataModels;
using System.Collections.ObjectModel;
using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Security.Claims;
using static System.Net.HttpStatusCode;

namespace MinimalAPI.Services.Customers;

public class CustomersActionValidationService : ICustomersActionValidationService
{
	private static Dictionary<string, string> _customerUniquePropertiesErrors = new()
	{
		{ "mail", "That email address is already in use." }
	};

	private readonly IUnitOfWork _worker;
	public CustomersActionValidationService(IUnitOfWork worker)
	{
		_worker = worker;
	}

	public async Task<ValidationResult<IEnumerable<Customer>>> GetCustomersAsync()
	{
		var canWork = await _worker.BeginWork<IEnumerable<Customer>>(false);
		if(canWork.ResultCode != Continue)
			return canWork;

		var customers = await _worker.Customers.GetCustomersAsync();

		if(customers == null)
			return new ValidationResult<IEnumerable<Customer>> { 
				ResultCode = InternalServerError, 
				ErrorMessage = "Could not retreive Customers." 
			};

		else if(customers.Count() == 0)
			return new ValidationResult<IEnumerable<Customer>> { 
				ResultCode = NoContent 
			};

		else
			return new ValidationResult<IEnumerable<Customer>> { 
				ResultCode = OK, 
				ResultValue = customers 
			};
	}

	public async Task<ValidationResult<Customer>> CreateCustomerAsync(ClaimsPrincipal user, Customer customer)
	{
		if(!(user.IsInRole(Role.Admin.ToString()) || user.FindFirst(JwtRegisteredClaimNames.Email)?.Value == customer.Email))
			return new ValidationResult<Customer> { 
				ResultCode = Unauthorized 
			};

		var canWork = await _worker.BeginWork<Customer>(true);
		if(canWork.ResultCode != Continue)
			return canWork;

		try
		{
			var newCustomer = await _worker.Customers.CreateCustomerAsync(customer);
			var changes = await _worker.SaveChangesAsync<Customer>();

			if(changes.ResultCode.IsSuccessCode())
			{
				changes.ResultValue = newCustomer;
				return changes;
			}

			else
				return changes;
		}
		catch(SqlException se)
		{
			if(ErrorHelper.DuplicateSqlErrors.Contains(se.Number)) // Unique constraint errors
				return await ErrorHelper.RollbackOnSqlServerDuplicateError<Customer>(_worker, se, _customerUniquePropertiesErrors);
			else
				return await ErrorHelper.RollbackOnSqlServerError<Customer>(_worker, se, "creating the Customer");
		}
		catch(Exception e)
		{
			return await ErrorHelper.RollbackOnServerError<Customer>(_worker, e);
		}
	}

	public async Task<ValidationResult<Customer>> GetCustomerAsync(ClaimsPrincipal user, int id)
	{
		if(!(user.IsInRole(Role.Admin.ToString()) || user.FindFirst("CustomerId")?.Value == id.ToString()))
			return new ValidationResult<Customer> { 
				ResultCode = Unauthorized 
			};

		var canWork = await _worker.BeginWork<Customer>(false);
		if(canWork.ResultCode != Continue)
			return canWork;

		var repo = _worker.Customers;
		var customer = await repo.GetCustomerAsync(id);

		return new ValidationResult<Customer> { 
			ResultCode = customer == null ? NotFound : OK, 
			ResultValue = customer 
		};
	}

	public async Task<ValidationResult<Customer>> UpdateCustomerAsync(ClaimsPrincipal user, int id, Dictionary<string, string> updates)
	{
		if(!(user.IsInRole(Role.Admin.ToString()) || user.FindFirst("CustomerId")?.Value == id.ToString()))
			return new ValidationResult<Customer> { 
				ResultCode = Unauthorized 
			};

		if(updates == null || updates.Count == 0)
			return new ValidationResult<Customer> { 
				ResultCode = BadRequest, 
				ErrorMessage = "No properties were provided" 
			};

		var canWork = await _worker.BeginWork<Customer>(true);
		if(canWork.ResultCode != Continue)
			return canWork;

		try
		{
			var updatedCustomer = await _worker.Customers.UpdateCustomerAsync(id, updates);

			if(updatedCustomer == null)
				return new ValidationResult<Customer> {
					ResultCode = NotFound,
					ErrorMessage = $"Customer with id {id} could not be found." 
				};

			var changes = await _worker.SaveChangesAsync<Customer>();

			if(changes.ResultCode.IsSuccessCode())
			{
				changes.ResultValue = updatedCustomer;
				return changes;
			}

			else
				return changes;
		}
		catch(SqlException se)
		{
			if(ErrorHelper.DuplicateSqlErrors.Contains(se.Number)) // Unique constraint errors
				return await ErrorHelper.RollbackOnSqlServerDuplicateError<Customer>(_worker, se, _customerUniquePropertiesErrors);

			else
				return await ErrorHelper.RollbackOnSqlServerError<Customer>(_worker, se, "updating the Customer");
		}
		catch(Exception e)
		{
			return await ErrorHelper.RollbackOnServerError<Customer>(_worker, e);
		}
	}

	public async Task<ValidationResult<int>> DeleteCustomerAsync(ClaimsPrincipal user, int id)
	{
		if(!(user.IsInRole(Role.Admin.ToString()) || user.FindFirst("CustomerId")?.Value == id.ToString()))
			return new ValidationResult<int> { 
				ResultCode = Unauthorized 
			};

		var canWork = await _worker.BeginWork<int>(true);
		if(canWork.ResultCode != Continue)
			return canWork;

		try
		{
			var success = await _worker.Customers.DeleteCustomerAsync(id);

			if(!success)
				return new ValidationResult<int> {
					ResultCode = NotFound,
					ErrorMessage = $"Customer with id {id} could not be found." 
				};

			return await _worker.SaveChangesAsync<int>();
		}
		catch(SqlException se)
		{
			return await ErrorHelper.RollbackOnSqlServerError<int>(_worker, se, "deleting the Customer");
		}
		catch(Exception e)
		{
			return await ErrorHelper.RollbackOnServerError<int>(_worker, e);
		}
	}

	public async Task<ValidationResult<IEnumerable<Order>>> GetOrdersAsync(ClaimsPrincipal user, int id)
	{
		if(!(user.IsInRole(Role.Admin.ToString()) || user.FindFirst("CustomerId")?.Value == id.ToString()))
			return new ValidationResult<IEnumerable<Order>> { 
				ResultCode = Unauthorized 
			};

		var canWork = await _worker.BeginWork<IEnumerable<Order>>(false);
		if(canWork.ResultCode != Continue)
			return canWork;

		var orders = await _worker.Orders.FindOrdersAsync(o => o.CustomerId == id);

		if(orders == null)
			return new ValidationResult<IEnumerable<Order>> { 
				ResultCode = NotFound 
			};

		else if(orders.Count() == 0)
			return new ValidationResult<IEnumerable<Order>> { 
				ResultCode = NoContent 
			};

		else
			return new ValidationResult<IEnumerable<Order>> { 
				ResultCode = OK, 
				ResultValue = orders 
			};
	}
}
