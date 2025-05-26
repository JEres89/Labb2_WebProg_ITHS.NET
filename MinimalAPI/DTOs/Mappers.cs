using MinimalAPI.DataModels;
using MinimalAPI.DTOs.Requests.Customers;
using MinimalAPI.DTOs.Requests.Products;
using MinimalAPI.DTOs.Responses.Customers;
using MinimalAPI.DTOs.Responses.Orders;
using MinimalAPI.DTOs.Responses.Products;
using System.Reflection.Metadata;
using System.Linq;
using MinimalAPI.DTOs.Requests.Orders;

namespace MinimalAPI.DTOs;

public static class ProductMapping
{
	public const string PRODUCTS_PATH = "/api/products/";

	public static ProductResponse ToProductResponse(this Product product)
	{
		return new ProductResponse
		{
			Id = product.Id,
			Name = product.Name,
			Description = product.Description,
			Price = product.Price,
			Category = product.Category,
			Status = product.Status,
			Stock = product.Stock
		};
	}

	public static ProductGetAllResponse ToProductsResponse(this IEnumerable<Product> products)
	{
		return new ProductGetAllResponse
		{
			Products = products.Select(ToProductResponse)
		};
	}

	public static Product ToProduct(this ProductCreateRequest request)
	{
		return new Product
		{
			Name = request.Name,
			Description = request.Description,
			Category = request.Category,
			Price = request.Price,
			Status = ProductStatus.Active,
			Stock = request.Stock
		};
	}

	//public static Product ToProduct(this ProductReplaceRequest request, int id)
	//{
	//	return new Product
	//	{
	//		Id = id,
	//		Name = request.Name,
	//		Description = request.Description,
	//		Category = request.Category,
	//		Price = request.Price,
	//		Status = request.Status,
	//		Stock = request.Stock
	//	};
	//}

	public static IEnumerable<ProductOrderResponse> ToProductOrdersResponse(this IEnumerable<OrderProduct> request)
	{
		return from productOrder in request
				select new ProductOrderResponse
				{
					Path = OrderMapping.ORDERS_PATH+productOrder.OrderId,
					Count = productOrder.Count,
					Price = productOrder.Price,
					OrderId = productOrder.OrderId
				};
	}

}

public static class CustomerMapping
{
	public const string CUSTOMERS_PATH = "/api/customers/";

	public static CustomerResponse ToCustomerResponse(this Customer customer)
	{
		return new CustomerResponse
		{
			Id = customer.Id,
			FirstName = customer.FirstName,
			LastName = customer.LastName,
			Email = customer.Email,
			Phone = customer.Phone,
			Address = customer.Address,
			Orders = customer.Orders?.Select(OrderMapping.ToSlimOrderResponse)
		};
	}

	public static CustomerGetAllResponse ToCustomersResponse(this IEnumerable<Customer> customers)
	{
		return new CustomerGetAllResponse
		{
			Customers = customers.Select(ToCustomerResponse)
		};
	}

	public static Customer ToCustomer(this CustomerCreateRequest request)
	{
		return new Customer
		{
			FirstName = request.FirstName,
			LastName = request.LastName,
			Email = request.Email,
			Phone = request.Phone,
			Address = request.Address
		};
	}

}

public static class OrderMapping
{
	public const string ORDERS_PATH = "/api/orders/";
	public static OrderResponse ToOrderResponse(this Order order)
	{
		var products = order.ToProductCountArray();
		return new OrderResponse
		{
			Id = order.Id,
			CustomerId = order.CustomerId,
			Status = order.Status,
			Products = products.Length == 0 ? null : products
		};
	}
	public static OrderResponse ToSlimOrderResponse(this Order order)
	{
		return new OrderResponse
		{
			Id = order.Id,
			CustomerId = order.CustomerId,
			Status = order.Status,
			Products = null
		};
	}

	public static OrdersResponse ToOrdersResponse(this IEnumerable<Order> orders)
	{
		return new OrdersResponse
		{
			Orders = orders.Select(ToOrderResponse)
		};
	}

	public static Order ToOrder(this OrderCreateRequest request)
	{
		return new Order
		{
			CustomerId = request.CustomerId,
			Products = new(),
			Status = OrderStatus.New
		};
	}

	public static IEnumerable<OrderProductResponse> ToOrderProductsResponse(this Order order)
	{
		return from product in order.Products
				select new OrderProductResponse
				{
					Path = ProductMapping.PRODUCTS_PATH + product.ProductId,
					Count = product.Count,
					Price = product.Price,
					Product = product.Product!.ToProductResponse()
				};
	}

	public static OrderProductsChangeResponse ToOrderProductsChangeResponse(this Order order/*, OrderProductsChangeRequest request*/)
	{
		//var newProducts = request.Products;
		//for(int i = 0; i < newProducts.Length; i++)
		//{
		//	var newP = newProducts[i];
		//	OrderProduct? oldP;
		//	if((oldP = order.Products.FirstOrDefault(p => p.ProductId == newP[0])) != null)
		//	{
		//		oldP.Count = newP[1];
		//	}
		//	else
		//	{
		//		order.Products.Add(new OrderProduct { ProductId = newP[0], Count = newP[1] });
		//	}
		//}

		return new OrderProductsChangeResponse { Products = order.ToProductCountArray() };
	}

	private static int[][] ToProductCountArray(this Order order)
	{
		if(order.Products == null)
			return Array.Empty<int[]>();

		return (from product in order.Products
				select new int[] { product.ProductId, product.Count }).ToArray();
	}

	//private static List<OrderProduct> ToOrderProductList(this int[][] products, int orderId, Func<int, decimal> priceSource)
	//{
	//	return (from product in products
	//			select new OrderProduct { 
	//				ProductId = product[0], Count = product[1], Price = priceSource(product[0]) }).ToList();
	//}

}
