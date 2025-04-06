using MinimalAPI.Auth;
using MinimalAPI.DataModels;
using MinimalAPI.DTOs.Requests.Orders;
using System.Threading.Channels;
using static MinimalAPI.Services.ValidationResultCode;

namespace MinimalAPI.Services.Orders;

public class OrdersActionValidationService : IOrdersActionValidationService
{
	private readonly IUnitOfWork _worker;
	public OrdersActionValidationService(IUnitOfWork worker)
	{
		_worker = worker;
	}

	public async Task<ValidationResult<IEnumerable<Order>>> GetOrdersAsync(WebUser? user)
	{
		if(user == null || user.Role != Role.Admin)
			return new ValidationResult<IEnumerable<Order>> { ResultCode = Unauthorized };

		var repo = _worker.Orders;
		var orders = await repo.GetOrdersAsync();

		if(orders == null)
		{
			return new ValidationResult<IEnumerable<Order>>
			{
				ResultCode = Failed,
				ErrorMessage = "Could not retrieve Orders."
			};
		}
		else if(!orders.Any())
		{
			return new ValidationResult<IEnumerable<Order>> { ResultCode = Success };
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

	public async Task<ValidationResult<Order>> CreateOrderAsync(WebUser? user, Order order)
	{
		if(user == null || (user.CustomerId != order.CustomerId && user.Role != Role.Admin))
			return new ValidationResult<Order> { ResultCode = Unauthorized };

		var repo = _worker.Orders;
		var newOrder = await repo.CreateOrderAsync(order);
		if(0 == await _worker.SaveChangesAsync())
		{
			return new ValidationResult<Order>
			{
				ResultCode = Failed,
				ErrorMessage = "Order could not be created."
			};
		}

		return new ValidationResult<Order>
		{
			ResultCode = Success,
			ResultValue = newOrder
		};
	}

	public async Task<ValidationResult<Order>> GetOrderAsync(WebUser? user, int id)
	{
		return await GetOrderAsync(user, id, false);

		//if(user == null)
		//	return new ValidationResult<Order> { ResultCode = Unauthorized };
		//else if(user.Role != Role.Admin)
		//{
		//	if(user.Customer == null)
		//	{
		//		var customer = await _worker.Customers.GetCustomerAsync(user.CustomerId);
		//		if(customer == null)
		//			return new ValidationResult<Order> { ResultCode = Unauthorized };

		//		if(user.Customer == null)
		//			user.Customer = customer;

		//		if(customer.)
		//	}
		//	var order = await _worker.Orders.GetOrderAsync(id);
		//	if(order != null && order.CustomerId == user.CustomerId)
		//		return new ValidationResult<Order> { ResultCode = Success, ResultValue = order };
		//	else
		//	{
		//		return new ValidationResult<Order> { ResultCode = Unauthorized };
		//	}
		//}

		//var repo = _worker.Orders;
		//var order = await repo.GetOrderAsync(id);

		//if(user.Role != Role.Admin || user.CustomerId != order.CustomerId)
		//	return new ValidationResult<Order> { ResultCode = Unauthorized };

		//return new ValidationResult<Order>
		//{
		//	ResultCode = order == null ? NotFound : Success,
		//	ResultValue = order
		//};
	}
	private async Task<ValidationResult<Order>> GetOrderAsync(WebUser? user, int id, bool withProducts)
	{
		if(user == null)
			return new ValidationResult<Order> { ResultCode = Unauthorized };

		var repo = _worker.Orders;
		Order? order;
		if(user.Role == Role.Admin)
			order = await repo.GetOrderAsync(id, withProducts);
		else
		{
			order = await repo.GetOrderAsync(id);
			if(order != null && order.CustomerId == user.CustomerId)
				if(withProducts)
					order = await repo.GetOrderAsync(id, true);
			else
				return new ValidationResult<Order> { ResultCode = Unauthorized };
		}

		if(order == null)
			return new ValidationResult<Order> { ResultCode = NotFound };

		return new ValidationResult<Order>
		{
			ResultCode = Success,
			ResultValue = order
		};
	}

	public async Task<ValidationResult<Order>> UpdateOrderAsync(WebUser? user, int id, OrderStatus status)
	{
		if(user == null || user.Role != Role.Admin)
			return new ValidationResult<Order> { ResultCode = Unauthorized };

		var repo = _worker.Orders;
		var order = await repo.GetOrderAsync(id);

		if(order == null)
			return new ValidationResult<Order> { ResultCode = NotFound };

		order.Status = status;

		var updatedOrder = await repo.UpdateOrderStatusAsync(id, status);

		if(0 == await _worker.SaveChangesAsync())
		{
			return new ValidationResult<Order>
			{
				ResultCode = Failed,
				ErrorMessage = "No changes were made."
			};
		}
		return new ValidationResult<Order>
		{
			ResultCode = Success,
			ResultValue = updatedOrder
		};
	}

	public async Task<ValidationResultCode> DeleteOrderAsync(WebUser? user, int id)
	{
		if(user == null)
			return Unauthorized;

		var repo = _worker.Orders;
		bool success = false;

		if(user.Role == Role.Admin)
			success = await repo.DeleteOrderAsync(id);
		else
		{
			var order = await repo.GetOrderAsync(id);
			if(order != null && order.CustomerId == user.CustomerId)
				success = await repo.DeleteOrderAsync(id);
			else
				return Unauthorized;
		}

		if(!success)
			return NotFound;

		var changes = await _worker.SaveChangesAsync();

		return changes > 0 ? Success : Failed;
	}

	public async Task<ValidationResult<Order>> GetProductsAsync(WebUser? user, int id)
	{
		return await GetOrderAsync(user, id, true);
	}

	public async Task<ValidationResult<Order>> UpdateProductsAsync(WebUser? user, int id, IEnumerable<int[]> setProducts)
	{
		if(user == null)
			return new ValidationResult<Order> { ResultCode = Unauthorized };

		var repo = _worker.Orders;
		Order? order;
		if(user.Role == Role.Admin)
			order = await repo.GetOrderAsync(id, true);
		else
		{
			order = await repo.GetOrderAsync(id);
			if(order != null && order.CustomerId == user.CustomerId)
				order = await repo.GetOrderAsync(id, true);
			else
				return new ValidationResult<Order> { ResultCode = Unauthorized };
		}

		if(order == null)
			return new ValidationResult<Order> { ResultCode = NotFound };

		var updatedOrder = await repo.UpdateProductsAsync(id, setProducts);

		if(0 == await _worker.SaveChangesAsync())
		{
			return new ValidationResult<Order>
			{
				ResultCode = Failed,
				ErrorMessage = "No changes were made."
			};
		}
		return new ValidationResult<Order>
		{
			ResultCode = Success,
			ResultValue = updatedOrder
		};
	}

	public async Task<ValidationResult<Order>> SetProductsAsync(WebUser? user, int id, IEnumerable<int[]> setProducts)
	{
		if(user == null)
			return new ValidationResult<Order> { ResultCode = Unauthorized };

		var repo = _worker.Orders;
		Order? order;
		if(user.Role == Role.Admin)
			order = await repo.GetOrderAsync(id, true);
		else
		{
			order = await repo.GetOrderAsync(id);
			if(order != null && order.CustomerId == user.CustomerId)
				order = await repo.GetOrderAsync(id, true);
			else
				return new ValidationResult<Order> { ResultCode = Unauthorized };
		}

		var updatedOrder = await repo.SetProductsAsync(id, setProducts);

		if(0 == await _worker.SaveChangesAsync())
		{
			return new ValidationResult<Order>
			{
				ResultCode = Failed,
				ErrorMessage = "No changes were made."
			};
		}
		return new ValidationResult<Order>
		{
			ResultCode = Success,
			ResultValue = updatedOrder
		};
	}
}
