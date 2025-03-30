using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using MinimalAPI.Auth;
using MinimalAPI.DTOs.Requests.Customers;
using MinimalAPI.DTOs.Responses.Customers;
using MinimalAPI.DTOs.Responses.Orders;
using MinimalAPI.Mappers;
using MinimalAPI.Services;
using static MinimalAPI.Services.ValidationResultCode;

namespace MinimalAPI.Endpoints.Customers;

public static class CustomersEndpoints
{
	[HttpGet(Name = "GetCustomers")]
	public static async Task<Results<Ok<CustomersResponse>,UnauthorizedHttpResult, NoContent,BadRequest<string?>,StatusCodeHttpResult>> GetCustomers(ICustomersActionValidationService validation, WebUser? user)
	{
		var result = await validation.GetCustomersAsync(user);
		switch(result.ResultCode)
		{
			case Success:
				return TypedResults.Ok(result.ResultValue!.ToCustomersResponse());

			case Unauthorized:
				return TypedResults.Unauthorized();

			case ValidationResultCode.NotFound:
				return TypedResults.NoContent();

			case Failed:
				return TypedResults.BadRequest<string?>(result.ErrorMessage);

			default:
				return TypedResults.StatusCode(StatusCodes.Status500InternalServerError);
		}
	}

	[HttpGet("{id}", Name = "GetCustomer")]
	public static async Task<Results<Ok<CustomerResponse>,UnauthorizedHttpResult, NotFound,BadRequest<string?>,StatusCodeHttpResult>> GetCustomer(ICustomersActionValidationService validation, WebUser? user, [FromRoute] int id)
	{
		var result = await validation.GetCustomerAsync(user, id);
		switch(result.ResultCode)
		{
			case Success:
				return TypedResults.Ok(result.ResultValue!.ToCustomerResponse());

			case Unauthorized:
				return TypedResults.Unauthorized();

			case ValidationResultCode.NotFound:
				return TypedResults.NotFound();

			case Failed:
				return TypedResults.BadRequest<string?>(result.ErrorMessage);

			default:
				return TypedResults.StatusCode(StatusCodes.Status500InternalServerError);
		}
	}

	[HttpPost(Name = "CreateCustomer")]
	public static async Task<Results<CreatedAtRoute<CustomerResponse>, UnauthorizedHttpResult, NotFound, BadRequest<string?>, StatusCodeHttpResult>> CreateCustomer(ICustomersActionValidationService validation, WebUser? user, [FromBody] CustomerCreateRequest request)
	{
		var customer = request.ToCustomer();
		var result = await validation.CreateCustomerAsync(user, customer);
		switch(result.ResultCode)
		{
			case Success:
				return TypedResults.CreatedAtRoute(
					customer!.ToCustomerResponse(), 
					routeName: "GetCustomer", 
					new { id = customer!.Id}
					);

			case Unauthorized:
				return TypedResults.Unauthorized();

			case ValidationResultCode.NotFound:
				return TypedResults.NotFound();

			case Failed:
				return TypedResults.BadRequest<string?>(result.ErrorMessage);

			default:
				return TypedResults.StatusCode(StatusCodes.Status500InternalServerError);
		}
	}

	[HttpPatch("{id}", Name = "UpdateCustomer")]
	public static async Task<Results<Ok<CustomerResponse>, UnauthorizedHttpResult, NotFound, BadRequest<string?>, StatusCodeHttpResult>> UpdateCustomer(ICustomersActionValidationService validation, WebUser? user, [FromRoute] int id, [FromBody] CustomerUpdateRequest request)
	{
		var result = await validation.UpdateCustomerAsync(user, id, request);

		switch(result.ResultCode)
		{
			case Success:
				return TypedResults.Ok(result.ResultValue!.ToCustomerResponse());

			case Unauthorized:
				return TypedResults.Unauthorized();

			case ValidationResultCode.NotFound:
				return TypedResults.NotFound();

			case Failed:
				return TypedResults.BadRequest<string?>(result.ErrorMessage);

			default:
				return TypedResults.StatusCode(StatusCodes.Status500InternalServerError);
		}
	}

	[HttpDelete("{id}", Name = "DeleteCustomer")]
	public static async Task<Results<NoContent, UnauthorizedHttpResult, NotFound, BadRequest, StatusCodeHttpResult>> DeleteCustomer(ICustomersActionValidationService validation, WebUser? user, [FromRoute] int id)
	{		
		switch(await validation.DeleteCustomerAsync(user, id))
		{
			case Success:
				return TypedResults.NoContent();

			case Unauthorized:
				return TypedResults.Unauthorized();

			case ValidationResultCode.NotFound:
				return TypedResults.NotFound();

			case Failed:
				return TypedResults.BadRequest();

			default:
				return TypedResults.StatusCode(StatusCodes.Status500InternalServerError);
		}
	}

	[HttpGet("{id}/orders", Name = "GetCustomerOrders")]
	public static async Task<Results<NoContent, Ok<OrdersResponse>, UnauthorizedHttpResult, NotFound, BadRequest<string?>, StatusCodeHttpResult>> GetOrders(ICustomersActionValidationService validation, WebUser? user, [FromRoute] int id)
	{
		var result = await validation.GetOrdersAsync(user, id);

		switch(result.ResultCode)
		{
			case Success:
				if(result.ResultValue == null)
				{
					return TypedResults.NoContent();
				}
				return TypedResults.Ok(result.ResultValue.ToOrdersResponse());

			case Unauthorized:
				return TypedResults.Unauthorized();

			case ValidationResultCode.NotFound:
				return TypedResults.NotFound();

			case Failed:
				return TypedResults.BadRequest<string?>(result.ErrorMessage);

			default:
				return TypedResults.StatusCode(StatusCodes.Status500InternalServerError);
		}
	}
}
