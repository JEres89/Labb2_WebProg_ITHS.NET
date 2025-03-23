using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using MinimalAPI.DataModels;
using MinimalAPI.DTOs.Requests.Customers;
using MinimalAPI.DTOs.Responses.Customers;
using MinimalAPI.DTOs.Responses.Orders;
using MinimalAPI.Mappers;
using MinimalAPI.Services;
using System.Linq.Expressions;

namespace MinimalAPI.Endpoints.Customers;

public static class CustomersEndpoints
{
	public static async Task<CustomersResponse> GetCustomers(ICustomersRepository repository) 
		=> (await repository.GetCustomersAsync()).ToCustomersResponse();

	public static async Task<CustomerResponse> GetCustomer(ICustomersRepository repository, [FromRoute] int id) 
		=> (await repository.GetCustomerAsync(id)).ToCustomerResponse();

	public static async Task<CustomerResponse> CreateCustomer(IUnitOfWork worker, [FromBody] CustomerCreateRequest request)
	{
		var customer = request.ToCustomer();
		var createdCustomer = await worker.Customers.CreateCustomerAsync(customer);
		await worker.SaveChangesAsync();

		if(customer.Id == 0)
		{
			throw new Exception("Failed to create customer");
		}

		return createdCustomer.ToCustomerResponse();
	}

	public static async Task<CustomerResponse> UpdateCustomer(IUnitOfWork worker, [FromRoute] int id, [FromBody] CustomerUpdateRequest request)
	{
		var repo = worker.Customers;
		var customer = await repo.GetCustomerAsync(id);
		if(customer == null)
		{
			throw new Exception("Customer not found");
		}

		foreach(var prop in request)
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

		var updatedCustomer = await worker.Customers.UpdateCustomerAsync(id, customer);
		await worker.SaveChangesAsync();
		if(updatedCustomer == null)
		{
			throw new Exception("Failed to update customer");
		}
		return updatedCustomer.ToCustomerResponse();
	}

	public static async Task<bool> DeleteCustomer(IUnitOfWork worker, [FromRoute] int id)
	{
		var repo = worker.Customers;
		var success = await repo.DeleteCustomerAsync(id);
		var count = await worker.SaveChangesAsync();
		return success && count > 0;
	}

	public static async Task<OrdersResponse> GetOrders(IUnitOfWork worker, [FromRoute] int id)
	{
		var customer = await worker.Customers.GetCustomerAsync(id);
		if(customer == null)
		{
			throw new Exception("Customer not found");
		}
		var orders = customer.Orders;
		if(orders == null)
		{
			throw new Exception("Orders not found");
		}
		return new OrdersResponse
		{
			Orders = orders.Select(o => o.ToOrderResponse())
		};
	}
}
