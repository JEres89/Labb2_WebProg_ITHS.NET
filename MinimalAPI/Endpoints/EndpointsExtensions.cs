// Gray usings are not used in the code, but are used in the project for better readability
using Microsoft.AspNetCore.Http.HttpResults;
using MinimalAPI.Auth;
using MinimalAPI.DTOs.Requests.Customers;
using MinimalAPI.DTOs.Requests.Orders;
using MinimalAPI.DTOs.Requests.Products;
using MinimalAPI.DTOs.Responses.Customers;
using MinimalAPI.DTOs.Responses.Orders;
using MinimalAPI.DTOs.Responses.Products;

using MinimalAPI.Services.Customers;
using MinimalAPI.Services.Orders;
using MinimalAPI.Services.Products;

namespace MinimalAPI.Endpoints;

public static class EndpointsExtensions
{
	public static IEndpointRouteBuilder MapApiEndpoints(this IEndpointRouteBuilder app)
	{
		app.MapGroup("/api/customers")/*.RequireAuthorization()*/.MapCustomerEndpoints();
		app.MapGroup("/api/products").MapProductEndpoints();
		app.MapGroup("/api/orders")/*.RequireAuthorization()*/.MapOrderEndpoints();
		return app;
	}

	public static IEndpointRouteBuilder MapCustomerEndpoints(this RouteGroupBuilder app)
	{
		app.MapGet("/", CustomersEndpoints.GetCustomers)/*.RequireAuthorization("Administrator")*/;

		app.MapGet("/{id}", CustomersEndpoints.GetCustomer).WithName("GetCustomer");

		app.MapPost("/", CustomersEndpoints.CreateCustomer);

		app.MapPatch("/{id}", CustomersEndpoints.UpdateCustomer);

		app.MapDelete("/{id}", CustomersEndpoints.DeleteCustomer)/*.RequireAuthorization("Administrator")*/;

		app.MapGet("/{id}/orders", CustomersEndpoints.GetOrders);

		return app;
	}
	public static IEndpointRouteBuilder MapProductEndpoints(this RouteGroupBuilder app)
	{
		app.MapGet("/", ProductsEndpoints.GetProducts);

		app.MapGet("/{id}", ProductsEndpoints.GetProduct).WithName(endpointName: "GetProduct");

		app.MapPost("/", ProductsEndpoints.CreateProduct)/*.RequireAuthorization("Administrator")*/;

		app.MapPatch("/{id}", ProductsEndpoints.UpdateProduct)/*.RequireAuthorization("Administrator")*/;

		app.MapDelete("/{id}", ProductsEndpoints.DeleteProduct)/*.RequireAuthorization("Administrator")*/;
		app.MapGet("/{id}/orders", ProductsEndpoints.GetOrders)/*.RequireAuthorization("Administrator")*/;

		return app;
	}
	public static IEndpointRouteBuilder MapOrderEndpoints(this RouteGroupBuilder app)
	{
		app.MapGet("/", OrdersEndpoints.GetOrders)/*.RequireAuthorization("Administrator")*/;

		app.MapGet("/{id}", OrdersEndpoints.GetOrder).WithName(endpointName: "GetOrder"); ;

		app.MapPost("/", OrdersEndpoints.CreateOrder);

		app.MapPatch("/{id}", OrdersEndpoints.UpdateOrder);

		app.MapDelete("/{id}", OrdersEndpoints.DeleteOrder);

		app.MapGet("/{id}/products", OrdersEndpoints.GetProducts);

		app.MapPatch("/{id}/products", OrdersEndpoints.SetProducts);

		app.MapPut("/{id}/products", OrdersEndpoints.ReplaceProducts);

		return app;
	}

}
