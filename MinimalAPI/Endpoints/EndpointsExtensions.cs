using MinimalAPI.DTOs.Responses.Customers;

namespace MinimalAPI.Endpoints;

public static class EndpointsExtensions
{
	public static IEndpointRouteBuilder MapApiEndpoints(this IEndpointRouteBuilder app)
	{
		app.MapGroup("/api/customers").RequireAuthorization().MapCustomerEndpoints();
		app.MapGroup("/api/products").RequireAuthorization().MapProductEndpoints();

		return app;
	}

	public static IEndpointRouteBuilder MapCustomerEndpoints(this RouteGroupBuilder app)
	{
		app.MapGet("/", CustomersEndpoints.GetCustomers).RequireAuthorization("Administrator");

		app.MapGet("/{id}", CustomersEndpoints.GetCustomer);

		app.MapPost("/", CustomersEndpoints.CreateCustomer);

		app.MapPatch("/{id}", CustomersEndpoints.UpdateCustomer);

		app.MapDelete("/{id}", CustomersEndpoints.DeleteCustomer).RequireAuthorization("Administrator");

		app.MapGet("/{id}/orders", CustomersEndpoints.GetOrders);

		return app;
	}
	public static IEndpointRouteBuilder MapProductEndpoints(this RouteGroupBuilder app)
	{
		app.MapGet("/", ProductsEndpoints.GetProducts);

		app.MapGet("/{id}", ProductsEndpoints.GetProduct);

		app.MapPost("/", ProductsEndpoints.CreateProduct).RequireAuthorization("Administrator");

		app.MapPatch("/{id}", ProductsEndpoints.UpdateProduct).RequireAuthorization("Administrator");

		app.MapDelete("/{id}", ProductsEndpoints.DeleteProduct).RequireAuthorization("Administrator");

		return app;
	}


}
