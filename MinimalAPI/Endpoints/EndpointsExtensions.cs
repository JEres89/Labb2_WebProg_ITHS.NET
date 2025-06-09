// Gray usings are not used in the code, but are used in the project for better readability
using Azure.Core;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.OpenApi.Models;
using MinimalAPI.Auth;
using MinimalAPI.DTOs.Requests.Auth;
using MinimalAPI.DTOs.Requests.Customers;
using MinimalAPI.DTOs.Requests.Orders;
using MinimalAPI.DTOs.Requests.Products;
using MinimalAPI.DTOs.Responses.Customers;
using MinimalAPI.DTOs.Responses.Orders;
using MinimalAPI.DTOs.Responses.Products;
using MinimalAPI.Services.Auth;
using MinimalAPI.Services.Customers;
using MinimalAPI.Services.Orders;
using MinimalAPI.Services.Products;

namespace MinimalAPI.Endpoints;

public static class EndpointsExtensions
{
	public static IEndpointRouteBuilder MapApiEndpoints(this IEndpointRouteBuilder app)
	{
		app.MapGroup("/api/customers")
			.RequireAuthorization()
			.MapCustomerEndpoints();

		app.MapGroup("/api/products")
			.MapProductEndpoints();

		app.MapGroup("/api/orders")
			.RequireAuthorization()
			.MapOrderEndpoints();

		app.MapGroup("/api/auth")
			.MapAuthEndpoints();

		return app;
	}
	//private static OpenApiOperation MapAuth(OpenApiOperation operation)
	//{
	//	var sec = new OpenApiSecurityRequirement();
	//	sec.Add(new OpenApiSecurityScheme()
	//	{ });
	//	operation.Security.Add(sec);
	//	return operation;
	//}
	public static IEndpointRouteBuilder MapCustomerEndpoints(this RouteGroupBuilder app)
	{
		app.MapGet("/", CustomersEndpoints.GetCustomers)
			.RequireAuthorization("Administrator")
			.WithName("GetCustomers");

		app.MapGet("/{id}", CustomersEndpoints.GetCustomer)
			.WithName("GetCustomer");

		app.MapPost("/", CustomersEndpoints.CreateCustomer)
			.WithName("CreateCustomer");

		app.MapPatch("/{id}", CustomersEndpoints.UpdateCustomer)
			.WithName("UpdateCustomer");

		app.MapDelete("/{id}", CustomersEndpoints.DeleteCustomer)
			.RequireAuthorization("Administrator")
			.WithName("DeleteCustomer");

		app.MapGet("/{id}/orders", CustomersEndpoints.GetOrders)
			.WithName("GetCustomerOrders");

		return app;
	}
	public static IEndpointRouteBuilder MapProductEndpoints(this RouteGroupBuilder app)
	{
		app.MapGet("/", ProductsEndpoints.GetProducts)
			.WithName("GetProducts").WithOpenApi();

		app.MapGet("/{id}", ProductsEndpoints.GetProduct)
			.WithName("GetProduct");

		app.MapPost("/", ProductsEndpoints.CreateProduct)
			.RequireAuthorization("Administrator")
			.WithName("CreateProduct").WithOpenApi();

		app.MapPatch("/{id}", ProductsEndpoints.UpdateProduct)
			.RequireAuthorization("Administrator")
			.WithName("UpdateProduct");

		app.MapDelete("/{id}", ProductsEndpoints.DeleteProduct)
			.RequireAuthorization("Administrator")
			.WithName("DeleteProduct");

		app.MapGet("/{id}/orders", ProductsEndpoints.GetOrders)
			.RequireAuthorization("Administrator")
			.WithName("GetProductOrders");

		return app;
	}
	public static IEndpointRouteBuilder MapOrderEndpoints(this RouteGroupBuilder app)
	{
		app.MapGet("/", OrdersEndpoints.GetOrders)
			.RequireAuthorization("Administrator")
			.WithName("GetOrders");

		app.MapGet("/{id}", OrdersEndpoints.GetOrder)
			.WithName("GetOrder");

		app.MapPost("/", OrdersEndpoints.CreateOrder)
			.WithName("CreateOrder");

		app.MapPatch("/{id}", OrdersEndpoints.UpdateOrder)
			.WithName("UpdateOrder");

		app.MapDelete("/{id}", OrdersEndpoints.DeleteOrder)
			.WithName("DeleteOrder");

		app.MapGet("/{id}/products", OrdersEndpoints.GetProducts)
			.WithName("GetOrderProducts");

		app.MapPatch("/{id}/products", OrdersEndpoints.SetProducts)
			.WithName("SetProducts");

		app.MapPut("/{id}/products", OrdersEndpoints.ReplaceProducts)
			.WithName("ReplaceProducts");

		return app;
	}

	public static void MapAuthEndpoints(this IEndpointRouteBuilder app)
	{
		#if DEBUG
		app.MapGet("/", (WebUser u) => TypedResults.Ok(TokenGenerator.GenerateToken(SecureSecretVault.GetJwtSecret(), u)))
			.WithName("AuthEndpoint")
			.WithTags("Auth");
		#endif

		app.MapPost("/register", AuthEndpoints.Register)
			.WithName("Register")
			.WithTags("Auth");

		app.MapPost("/login", AuthEndpoints.Login)/*(LoginRequest request, IAuthActionValidationService validation) => new { access_token = TokenGenerator.GenerateToken("DebugKey", 0, "asdasd", Role.Admin)}).AddEndpointFilter(async (context, next) =>*/
			.WithName("Login")
			.WithTags("Auth");

		app.MapPost("/logout", AuthEndpoints.Logout)
			.RequireAuthorization()
			.WithName("Logout")
			.WithTags("Auth");

		app.MapGet("/users", AuthEndpoints.GetUsers)
			.RequireAuthorization("Administrator")
			.WithName("GetUsers")
			.WithTags("Auth");

		app.MapGet("/users/{userEmail}", AuthEndpoints.GetUser)
			.RequireAuthorization()
			.WithName("GetUser")
			.WithTags("Auth");

		app.MapDelete("/users/{userEmail}", AuthEndpoints.DeleteUser)
			.RequireAuthorization("Administrator")
			.WithName("DeleteUser")
			.WithTags("Auth");
	}
}
