using Microsoft.Data.SqlClient;
using MinimalAPI.Auth;
using MinimalAPI.DataModels;
using System.Net;
using System.Security.Claims;
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

	public async Task<ValidationResult<IEnumerable<Order>>> GetOrdersAsync()
	{
		var canWork = await _worker.BeginWork<IEnumerable<Order>>(false);
		if(canWork.ResultCode != Continue)
			return canWork;

		var repo = _worker.Orders;
		var orders = await repo.GetOrdersAsync();

		if(orders == null)
			return new ValidationResult<IEnumerable<Order>> {
				ResultCode = InternalServerError,
				ErrorMessage = "Could not retrieve Orders." 
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

	/// <param name="products">[n][2] where each row is [productid, count]</param>
	public async Task<ValidationResult<Order>> CreateOrderAsync(ClaimsPrincipal user, Order order, int[][]? products)
	{
		bool isAdmin = user.IsInRole(Role.Admin.ToString());

		if(!(isAdmin || user.FindFirst("CustomerId")?.Value == order.CustomerId.ToString()))
			return new ValidationResult<Order> { 
				ResultCode = Unauthorized 
			};

		if(products == null || products.Length == 0)
			return new ValidationResult<Order> { 
				ResultCode = BadRequest, 
				ErrorMessage = "No products were provided." 
			};

		var canWork = await _worker.BeginWork<Order>(true);
		if(canWork.ResultCode != Continue)
			return canWork;

		if(isAdmin && !(await _worker.Customers.VerifyCustomer(order.CustomerId)))
			return new ValidationResult<Order> { 
				ResultCode = BadRequest, 
				ErrorMessage = $"Customer with id {order.CustomerId} does not exist." 
			};

		var repo = _worker.Orders;
		if(order.Status == OrderStatus.New)
		{
			var cart = (await repo.FindOrdersAsync(o => o.CustomerId == order.CustomerId && o.Status == OrderStatus.New)).FirstOrDefault();
			
			if(cart != null)
				return new ValidationResult<Order> {
					ResultCode = Conflict,
					ErrorMessage = "Customer already has an open cart.",
					ResultValue = cart 
				};
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

	public async Task<ValidationResult<Order>> GetOrderAsync(ClaimsPrincipal user, int orderId) => await GetOrderAsync(user, orderId, false);

	private async Task<ValidationResult<Order>> GetOrderAsync(ClaimsPrincipal user, int orderId, bool withProducts)
	{
		var canWork = await _worker.BeginWork<Order>(false);
		if(canWork.ResultCode != Continue)
			return canWork;

		var repo = _worker.Orders;

		Order? order;
		if(user.IsInRole(Role.Admin.ToString()))
			order = await repo.GetOrderAsync(orderId, withProducts);
		else
		{
			order = await repo.GetOrderAsync(orderId);
			if(order != null && order.CustomerId.ToString() == user.FindFirst("CustomerId")?.Value)
			{
				if(withProducts)
					order = await repo.GetOrderAsync(orderId, true);
			}
			else
				return new ValidationResult<Order> { 
					ResultCode = Unauthorized 
				};
		}

		if(order == null)
			return new ValidationResult<Order> { 
				ResultCode = NotFound,
				ErrorMessage = $"Order with id {orderId} could not be found." 
			};

		return new ValidationResult<Order> {
			ResultCode = OK,
			ResultValue = order 
		};
	}

	public async Task<ValidationResult<Order>> UpdateOrderAsync(ClaimsPrincipal user, int orderId, OrderStatus status)
	{
		if(user == null)
			return new ValidationResult<Order> { 
				ResultCode = Unauthorized 
			};

		var canWork = await _worker.BeginWork<Order>(true);
		if(canWork.ResultCode != Continue)
			return canWork;

		var repo = _worker.Orders;
		var order = await repo.GetOrderAsync(orderId);

		bool isAdmin = user.IsInRole(Role.Admin.ToString());

		if(order == null)
		{
			if(isAdmin)
				return new ValidationResult<Order> { 
					ResultCode = NotFound,
					ErrorMessage = $"Order with id {orderId} could not be found." 
				};

			return new ValidationResult<Order> { 
				ResultCode = Unauthorized 
			};
		}
		else if(!isAdmin && user.FindFirst("CustomerId")?.Value != order.CustomerId.ToString())
			return new ValidationResult<Order> { 
				ResultCode = Unauthorized 
			};

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

	public async Task<ValidationResult<int>> DeleteOrderAsync(ClaimsPrincipal user, int orderId)
	{
		var canWork = await _worker.BeginWork<int>(true);
		if(canWork.ResultCode != Continue)
			return canWork;

		try
		{
			var repo = _worker.Orders;
			bool success = false;

			if(user.IsInRole(Role.Admin.ToString()))
				success = await repo.DeleteOrderAsync(orderId);

			else
			{
				var order = await repo.GetOrderAsync(orderId);
				if(order != null && user.FindFirst("CustomerId")?.Value == order.CustomerId.ToString())
					success = await repo.DeleteOrderAsync(orderId);

				else
					return new ValidationResult<int> { 
						ResultCode = Unauthorized 
					};
			}

			if(!success)
				return new ValidationResult<int> { 
					ResultCode = NotFound,
					ErrorMessage = $"Order with id {orderId} could not be found." 
				};

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

	public async Task<ValidationResult<Order>> GetProductsAsync(ClaimsPrincipal user, int orderId)
	{
		return await GetOrderAsync(user, orderId, true);
	}

	/// <param name="setProducts">[n][2] where each row is [productid, count]</param>
	public async Task<ValidationResult<Order>> UpdateProductsAsync(ClaimsPrincipal user, int orderId, IEnumerable<int[]> setProducts, bool replace = false)
	{
		if(!setProducts.Any())
			return new ValidationResult<Order> {
				ResultCode = BadRequest,
				ErrorMessage = "Request was empty." 
			};

		var canWork = await _worker.BeginWork<Order>(true);
		if(canWork.ResultCode != Continue)
			return canWork;

		var repo = _worker.Orders;
		var order = await repo.GetOrderAsync(orderId);

		bool isAdmin = user.IsInRole(Role.Admin.ToString());
		if(order == null)
		{
			if(isAdmin)
				return new ValidationResult<Order> { 
					ResultCode = NotFound,
					ErrorMessage = $"Order with id {orderId} could not be found." 
				};

			return new ValidationResult<Order> { 
				ResultCode = Unauthorized 
			};
		}
		else if(!isAdmin && user.FindFirst("CustomerId")?.Value != order.CustomerId.ToString())
			return new ValidationResult<Order> { 
				ResultCode = Unauthorized 
			};

		else if(order.Status > OrderStatus.Processing)
			return new ValidationResult<Order> { 
				ResultCode = Forbidden, 
				ErrorMessage = $"Order is not in a state ({order.Status}) that allows product changes." 
			};

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
