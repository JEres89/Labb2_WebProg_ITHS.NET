using Microsoft.Data.SqlClient;
using MinimalAPI.Auth;
using MinimalAPI.DataModels;
using System.Net;
using System.Threading.Channels;
using static System.Net.HttpStatusCode;

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
			return new ValidationResult<IEnumerable<Order>> { 
				ResultCode = Unauthorized };

		var canWork = await _worker.BeginWork<IEnumerable<Order>>(false);
		if(canWork.ResultCode != Continue)
			return canWork;

		var repo = _worker.Orders;
		var orders = await repo.GetOrdersAsync();

		if(orders == null)
		{
			return new ValidationResult<IEnumerable<Order>> {
				ResultCode = InternalServerError,
				ErrorMessage = "Could not retrieve Orders." };
		}
		else if(orders.Count() == 0)
		{
			return new ValidationResult<IEnumerable<Order>> { 
				ResultCode = NoContent };
		}
		else
		{
			return new ValidationResult<IEnumerable<Order>> {
				ResultCode = OK,
				ResultValue = orders };
		}
	}

	/// <param name="products">[n][2] where each row is [productid, count]</param>
	public async Task<ValidationResult<Order>> CreateOrderAsync(WebUser? user, Order order, int[][]? products)
	{
		if(user == null || user.CustomerId != order.CustomerId && user.Role != Role.Admin)
			return new ValidationResult<Order> { 
				ResultCode = Unauthorized };

		if(products == null || products.Length == 0)
			return new ValidationResult<Order> { 
				ResultCode = BadRequest, 
				ErrorMessage = "No products were provided." };

		var canWork = await _worker.BeginWork<Order>(true);
		if(canWork.ResultCode != Continue)
			return canWork;

		if(user.Role == Role.Admin)
		{
			if(!(await _worker.Customers.VerifyCustomer(order.CustomerId)))
				return new ValidationResult<Order> { 
					ResultCode = BadRequest, 
					ErrorMessage = $"Customer with id {order.CustomerId} does not exist." };
		}

		var repo = _worker.Orders;
		if(order.Status == OrderStatus.New)
		{
			var cart = (await repo.FindOrdersAsync(o => o.CustomerId == order.CustomerId && o.Status == OrderStatus.New)).FirstOrDefault();
			if(cart != null)
			{
				return new ValidationResult<Order> {
					ResultCode = Conflict,
					ErrorMessage = "Customer already has an open cart.",
					ResultValue = cart };
			}
		}

		try
		{
			var newOrder = await repo.CreateOrderAsync(order, products);
			var changes = await _worker.SaveChangesAsync<Order>();

			if(changes.ResultCode.IsSuccessCode())
			{
				changes.ResultValue = newOrder;
				return changes;
			}

			else
				return changes;
		}
		catch(SqlException se)
		{
			return await ErrorHelper.RollbackOnSqlServerError<Order>(_worker, se, "creating the Order");
		}
		catch(Exception e)
		{
			return await ErrorHelper.RollbackOnServerError<Order>(_worker, e);
		}
	}

	public async Task<ValidationResult<Order>> GetOrderAsync(WebUser? user, int orderId) => await GetOrderAsync(user, orderId, false);

	private async Task<ValidationResult<Order>> GetOrderAsync(WebUser? user, int orderId, bool withProducts)
	{
		if(user == null)
			return new ValidationResult<Order> { 
				ResultCode = Unauthorized };

		var canWork = await _worker.BeginWork<Order>(false);
		if(canWork.ResultCode != Continue)
			return canWork;

		var repo = _worker.Orders;

		Order? order;
		if(user.Role == Role.Admin)
			order = await repo.GetOrderAsync(orderId, withProducts);
		else
		{
			order = await repo.GetOrderAsync(orderId);
			if(order != null && order.CustomerId == user.CustomerId)
			{
				if(withProducts)
					order = await repo.GetOrderAsync(orderId, true);
			}
			else
				return new ValidationResult<Order> { 
					ResultCode = Unauthorized };
		}

		if(order == null)
			return new ValidationResult<Order> { 
				ResultCode = NotFound,
				ErrorMessage = $"Order with id {orderId} could not be found." };

		return new ValidationResult<Order> {
			ResultCode = OK,
			ResultValue = order };
	}

	public async Task<ValidationResult<Order>> UpdateOrderAsync(WebUser? user, int orderId, OrderStatus status)
	{
		if(user == null)
			return new ValidationResult<Order> { 
				ResultCode = Unauthorized };

		var canWork = await _worker.BeginWork<Order>(true);
		if(canWork.ResultCode != Continue)
			return canWork;

		var repo = _worker.Orders;
		var order = await repo.GetOrderAsync(orderId);

		if(order == null)
		{
			if(user.Role == Role.Admin)
				return new ValidationResult<Order> { 
					ResultCode = NotFound,
					ErrorMessage = $"Order with id {orderId} could not be found." };

			return new ValidationResult<Order> { 
				ResultCode = Unauthorized };
		}
		else if(user.Role != Role.Admin && order.CustomerId != user.CustomerId)
			return new ValidationResult<Order> { 
				ResultCode = Unauthorized };

		try
		{
			var updatedOrder = await repo.UpdateOrderStatusAsync(orderId, status);
			var changes = await _worker.SaveChangesAsync<Order>();

			if(changes.ResultCode.IsSuccessCode())
			{
				changes.ResultValue = updatedOrder;
				return changes;
			}

			else
				return changes;
		}
		catch(SqlException se)
		{
			return await ErrorHelper.RollbackOnSqlServerError<Order>(_worker, se, "updating the Order");
		}
		catch(Exception e)
		{
			return await ErrorHelper.RollbackOnServerError<Order>(_worker, e);
		}
	}

	public async Task<ValidationResult<int>> DeleteOrderAsync(WebUser? user, int orderId)
	{
		if(user == null)
			return new ValidationResult<int> { 
				ResultCode = Unauthorized };

		var canWork = await _worker.BeginWork<int>(true);
		if(canWork.ResultCode != Continue)
			return canWork;

		try
		{
			var repo = _worker.Orders;
			bool success = false;

			if(user.Role == Role.Admin)
				success = await repo.DeleteOrderAsync(orderId);
			else
			{
				var order = await repo.GetOrderAsync(orderId);
				if(order != null && order.CustomerId == user.CustomerId)
					success = await repo.DeleteOrderAsync(orderId);
				else
					return new ValidationResult<int> { 
						ResultCode = Unauthorized };
			}

			if(!success)
				return new ValidationResult<int> { 
					ResultCode = NotFound,
					ErrorMessage = $"Order with id {orderId} could not be found." };

			return await _worker.SaveChangesAsync<int>();
		}
		catch(SqlException se)
		{
			return await ErrorHelper.RollbackOnSqlServerError<int>(_worker, se, "deleting the Order");
		}
		catch(Exception e)
		{
			return await ErrorHelper.RollbackOnServerError<int>(_worker, e);
		}
	}

	public async Task<ValidationResult<Order>> GetProductsAsync(WebUser? user, int orderId)
	{
		return await GetOrderAsync(user, orderId, true);
	}

	/// <param name="setProducts">[n][2] where each row is [productid, count]</param>
	public async Task<ValidationResult<Order>> UpdateProductsAsync(WebUser? user, int orderId, IEnumerable<int[]> setProducts, bool replace = false)
	{
		if(!setProducts.Any())
			return new ValidationResult<Order> {
				ResultCode = BadRequest,
				ErrorMessage = "Request was empty." };

		if(user == null)
			return new ValidationResult<Order> { 
				ResultCode = Unauthorized };

		var canWork = await _worker.BeginWork<Order>(true);
		if(canWork.ResultCode != Continue)
			return canWork;

		var repo = _worker.Orders;
		var order = await repo.GetOrderAsync(orderId);

		if(order == null)
		{
			if(user.Role == Role.Admin)
				return new ValidationResult<Order> { 
					ResultCode = NotFound,
					ErrorMessage = $"Order with id {orderId} could not be found." };

			return new ValidationResult<Order> { 
				ResultCode = Unauthorized };
		}
		else if(user.Role != Role.Admin && order.CustomerId != user.CustomerId)
			return new ValidationResult<Order> { 
				ResultCode = Unauthorized };
		else
			if(order.Status > OrderStatus.Processing)
			return new ValidationResult<Order> { 
				ResultCode = Forbidden, 
				ErrorMessage = $"Order is not in a state ({order.Status}) that allows product changes." };

		Order? updatedOrder = null;
		try
		{
			if(replace)
				updatedOrder = await repo.SetProductsAsync(orderId, setProducts);
			else
				updatedOrder = await repo.UpdateProductsAsync(orderId, setProducts);
			var changes = await _worker.SaveChangesAsync<Order>();

			if(changes.ResultCode.IsSuccessCode())
			{
				changes.ResultValue = updatedOrder;
				return changes;
			}

			else
				return changes;
		}
		catch(SqlException se)
		{
			return await ErrorHelper.RollbackOnSqlServerError<Order>(_worker, se, "updating the Products in the Order");
		}
		catch(Exception e)
		{
			return await ErrorHelper.RollbackOnServerError<Order>(_worker, e);
		}
	}
}
