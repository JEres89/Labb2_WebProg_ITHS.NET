using Microsoft.EntityFrameworkCore;
using MinimalAPI.DataModels;
using MinimalAPI.Services.Database;
using System.Linq.Expressions;

namespace MinimalAPI.Services.Orders;

public class OrdersRepository : IOrdersRepository
{
	private readonly ApiContext _context;
	public OrdersRepository(ApiContext context)
	{
		_context = context;
	}

	public async Task<IEnumerable<Order>> GetOrdersAsync()
	{
		return await _context.Orders.ToListAsync();
	}

	public async Task<Order> CreateOrderAsync(Order order, IEnumerable<int[]>? products)
	{
		if(products != null)
		{
			await foreach(var item in GenerateOrderProductsAsync(order.Id, products))
			{
				order.Products.Add(item);
			}
		}
		_context.Orders.Add(order);
		return order;
	}
	/// <summary>
	/// 
	/// </summary>
	/// <param name="withProducts">Specifies whether to include the <see cref="OrderProduct.Product"/> in <see cref="Order.Products"/>.</param>
	/// <returns></returns>
	public async Task<Order?> GetOrderAsync(int id, bool withProducts = false)
	{
		return withProducts 
			? await _context.Orders
					.Include(o => o.Products)
					.ThenInclude(p => p.Product)
					.FirstOrDefaultAsync(c => c.Id == id)

			: await _context.Orders.FindAsync(id);
	}

	public async Task<Order?> UpdateOrderStatusAsync(int id, OrderStatus status)
	{
		var order = await _context.Orders.FindAsync(id);
		if(order == null)
		{
			return null;
		}
		order.Status = status;

		return order;
		//var changes = await _context.SaveChangesAsync();

		//return changes > 0 ? order : null;
	}

	public async Task<bool> DeleteOrderAsync(int id)
	{
		var order = await GetOrderAsync(id);
		if(order == null)
		{
			return false;
		}

		_context.Orders.Remove(order);
		return true;
		//var changes = await _context.SaveChangesAsync();

		//return changes > 0;
	}

	public async Task<IEnumerable<Order>> FindOrdersAsync(Expression<Func<Order, bool>> orderMatch)
	{
		return await _context.Orders.Where(orderMatch).ToListAsync();
		//return await Task.FromResult(_context.Orders.AsEnumerable().Where(c => orderMatch(c)).ToList());
	}

	//public async Task<bool> EditProductAsync(int id, int productId, int amount)
	//{
	//	var productOrder = await _context.OrderProducts.FindAsync(id, productId);

	//	if(productOrder == null)
	//	{
	//		productOrder = new OrderProduct
	//		{
	//			OrderId = id,
	//			ProductId = productId,
	//			Count = amount,
	//			Price = _context.Products.Where(p => p.Id == productId).Select(p => p.Price).FirstOrDefault()
	//		};
	//		_context.Add(productOrder);
	//	}
	//	else
	//	{

	//	}

	//	await _context.SaveChangesAsync();
	//	return true;
	//}

	public async Task<Order?> UpdateProductsAsync(int id, IEnumerable<int[]> productChanges)
	{
		var order = await _context.Orders.FindAsync(id);
		if(order == null)
			return null;

		var productPrices = await _context.Products
			.Where(p => productChanges.Select(pc => pc[0]).Contains(p.Id))
			.ToDictionaryAsync(p => p.Id, p => p.Price);

		foreach(var product in productChanges)
		{
			var productOrder = order.Products.FirstOrDefault(p => p.ProductId == product[0]);
			if(!productPrices.TryGetValue(product[0], out var price))
			{
				continue;
			}
			if(productOrder != null)
			{
				if(product[1] == 0)
				{
					_context.OrderProducts.Remove(productOrder);
					continue;
				}
				productOrder.Count = product[1];
				productOrder.Price = price;
			}
			else
			{
				if(product[1] == 0)
					continue;

				order.Products.Add(new OrderProduct
				{
					OrderId = id,
					ProductId = product[0],
					Count = product[1],
					Price = price
				});
			}
		}
		//await _context.SaveChangesAsync();
		return order;
	}

	public async Task<Order?> SetProductsAsync(int id, IEnumerable<int[]> newProducts)
	{
		var order = await _context.Orders.FindAsync(id);
		if(order == null)
			return null;
		order.Products.Clear();

		await foreach(var item in GenerateOrderProductsAsync(id, newProducts))
		{
			order.Products.Add(item);
		}
		return order;
	}

	private async IAsyncEnumerable<OrderProduct> GenerateOrderProductsAsync(int orderId, IEnumerable<int[]> productChanges)
	{
		//var orderProducts = new List<OrderProduct>();
		var productPrices = await _context.Products
			.Where(p => productChanges.Select(pc => pc[0]).Contains(p.Id))
			.ToDictionaryAsync(p => p.Id, p => p.Price);

		foreach(var product in productChanges)
		{
			if(product[1] == 0)
				continue;
			yield return new OrderProduct
			{
				OrderId = orderId,
				ProductId = product[0],
				Count = product[1],
				Price = productPrices[product[0]]
			};
		}
		//return orderProducts;
	}

	public async Task<IEnumerable<OrderProduct>?> FindOrdersForProductAsync(Expression<Func<OrderProduct, bool>>? orderMatch, int productId)
	{
		//var orders = await _context.OrderProducts
		//		.Where(op => op.ProductId == productId)
		//		.ToListAsync();

		//if(orderMatch == null)
		//	return orders;

		//return orders
		//	.Where(orderMatch)
		//	.ToList();
		if(!await _context.Products.AnyAsync(p => p.Id == productId))
			return null;

		if(orderMatch == null)
			return await _context.OrderProducts
				.Where(op => op.ProductId == productId)
				.ToListAsync();

		return await _context.OrderProducts
			.Where(op => op.ProductId == productId).Where(orderMatch)
			.ToListAsync();
	}

	public void Dispose()
	{
		GC.SuppressFinalize(this);
	}
}
