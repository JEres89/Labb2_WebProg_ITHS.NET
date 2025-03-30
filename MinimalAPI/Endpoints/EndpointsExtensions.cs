using MinimalAPI.DTOs.Responses.Customers;
using MinimalAPI.Endpoints.Customers;

namespace MinimalAPI.Endpoints;

public static class EndpointsExtensions
{
	public static IEndpointRouteBuilder MapApiEndpoints(this IEndpointRouteBuilder app)
	{
		app.MapGroup("/api/customers").RequireAuthorization().MapCustomerEndpoints();

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

}
