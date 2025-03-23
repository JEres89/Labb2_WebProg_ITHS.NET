using MinimalAPI.DTOs.Responses.Customers;
using MinimalAPI.Endpoints.Customers;

namespace MinimalAPI.Endpoints;

public static class EndpointsExtensions
{
	public static IEndpointRouteBuilder MapApiEndpoints(this IEndpointRouteBuilder app)
	{
		

		return app;
	}

	public static IEndpointRouteBuilder MapCustomerEndpoints(this IEndpointRouteBuilder app)
	{
		app.MapGet("/api/customers", CustomersEndpoints.GetCustomers);//.RequireAuthorization("Administrator");

		app.MapGet("/api/customers/{id}", CustomersEndpoints.GetCustomer);//.RequireAuthorization("Administrator");

		app.MapPost("/api/customers", async (CustomerResponse customerResponse) =>
		{
			return Results.Ok(customerResponse);
		}).RequireAuthorization("Administrator");

		app.MapPut("/api/customers/{id}", async (int id, CustomerResponse customerResponse) =>
		{
			return Results.Ok(customerResponse);
		}).RequireAuthorization("Administrator");

		app.MapDelete("/api/customers/{id}", async (int id) =>
		{
			return Results.Ok(id);
		}).RequireAuthorization("Administrator");

		return app;
	}
}
