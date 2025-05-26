using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using MinimalAPI.Auth;
using MinimalAPI.DTOs;
using MinimalAPI.DTOs.Requests.Customers;
using MinimalAPI.DTOs.Responses.Customers;
using MinimalAPI.DTOs.Responses.Orders;
using MinimalAPI.Services.Customers;
using System.Net;

namespace MinimalAPI.Endpoints;

using GetCustomersResponseTask =
	Task<Results<
		Ok<CustomerGetAllResponse>,
		JsonHttpResult<string>,
		StatusCodeHttpResult>>;

using CreateCustomerResponseTask =
	Task<Results<
		CreatedAtRoute<CustomerResponse>,
		JsonHttpResult<string>,
		StatusCodeHttpResult>>;

using CustomerResponseTask =
	Task<Results<
		Ok<CustomerResponse>,
		JsonHttpResult<string>,
		StatusCodeHttpResult>>;

using DeleteCustomerResponseTask =
	Task<Results<
		NoContent,
		JsonHttpResult<string>,
		StatusCodeHttpResult>>;

using GetOrdersResponseTask =
	Task<Results<
		Ok<OrdersResponse>,
		JsonHttpResult<string>,
		StatusCodeHttpResult>>;

public static class CustomersEndpoints
{
	[HttpGet(Name = "GetCustomers")]
	[ProducesResponseType((int)HttpStatusCode.NoContent)]
	[ProducesResponseType((int)HttpStatusCode.Unauthorized)]
	[ProducesResponseType(typeof(string), (int)HttpStatusCode.ServiceUnavailable)]
	[ProducesResponseType(typeof(string), (int)HttpStatusCode.InternalServerError)]
	//[Authorize(Roles = "Admin")]
	public static async GetCustomersResponseTask GetCustomers(ICustomersActionValidationService validation, WebUser? user)
	{
		var result = await validation.GetCustomersAsync(user);
		switch(result.ResultCode)
		{
			case HttpStatusCode.OK:
				return TypedResults.Ok(result.ResultValue!.ToCustomersResponse());

			default:
				if(string.IsNullOrEmpty(result.ErrorMessage))
					return TypedResults.StatusCode((int)result.ResultCode);

				return TypedResults.Json(result.ErrorMessage, statusCode: (int)result.ResultCode);
		}
	}

	[HttpPost(Name = "CreateCustomer")]
	//[Authorize(Roles = "Admin")]
	public static async CreateCustomerResponseTask CreateCustomer(ICustomersActionValidationService validation, WebUser? user, [FromBody] CustomerCreateRequest request)
	{
		var customer = request.ToCustomer();
		var result = await validation.CreateCustomerAsync(user, customer);
		customer = result.ResultValue;
		switch(result.ResultCode)
		{
			case HttpStatusCode.OK:
				return TypedResults.CreatedAtRoute(
					value: customer!.ToCustomerResponse(),
					routeName: "GetCustomer",
					routeValues: new { id = customer!.Id }
				);

			default:
				if(string.IsNullOrEmpty(result.ErrorMessage))
					return TypedResults.StatusCode((int)result.ResultCode);

				return TypedResults.Json(result.ErrorMessage, statusCode: (int)result.ResultCode);
		}
	}

	[HttpGet("{id}", Name = "GetCustomer")]
	//[Authorize]
	public static async CustomerResponseTask GetCustomer(ICustomersActionValidationService validation, WebUser? user, [FromRoute] int id)
	{
		var result = await validation.GetCustomerAsync(user, id);
		switch(result.ResultCode)
		{
			case HttpStatusCode.OK:
				return TypedResults.Ok(result.ResultValue!.ToCustomerResponse());

			default:
				if(string.IsNullOrEmpty(result.ErrorMessage))
					return TypedResults.StatusCode((int)result.ResultCode);

				return TypedResults.Json(result.ErrorMessage, statusCode: (int)result.ResultCode);
		}
	}

	[HttpPatch("{id}", Name = "UpdateCustomer")]
	//[Authorize]
	public static async CustomerResponseTask UpdateCustomer(ICustomersActionValidationService validation, WebUser? user, [FromRoute] int id, [FromBody] CustomerUpdateRequest request)
	{
		var result = await validation.UpdateCustomerAsync(user, id, request);

		switch(result.ResultCode)
		{
			case HttpStatusCode.OK:
				return TypedResults.Ok(result.ResultValue!.ToCustomerResponse());

			default:
				if(string.IsNullOrEmpty(result.ErrorMessage))
					return TypedResults.StatusCode((int)result.ResultCode);

				return TypedResults.Json(result.ErrorMessage, statusCode: (int)result.ResultCode);
		}
	}

	[HttpDelete("{id}", Name = "DeleteCustomer")]
	//[Authorize]
	public static async DeleteCustomerResponseTask DeleteCustomer(ICustomersActionValidationService validation, WebUser? user, [FromRoute] int id)
	{
		var result = await validation.DeleteCustomerAsync(user, id);

		switch(result.ResultCode)
		{
			case HttpStatusCode.OK:
				return TypedResults.NoContent();

			default:
				if(string.IsNullOrEmpty(result.ErrorMessage))
					return TypedResults.StatusCode((int)result.ResultCode);

				return TypedResults.Json(result.ErrorMessage, statusCode: (int)result.ResultCode);
		}
	}

	[HttpGet("{id}/orders", Name = "GetCustomerOrders")]
	//[Authorize]
	public static async GetOrdersResponseTask GetOrders(ICustomersActionValidationService validation, WebUser? user, [FromRoute] int id)
	{
		var result = await validation.GetOrdersAsync(user, id);

		switch(result.ResultCode)
		{
			case HttpStatusCode.OK:
				return TypedResults.Ok(result.ResultValue!.ToOrdersResponse());

			default:
				if(string.IsNullOrEmpty(result.ErrorMessage))
					return TypedResults.StatusCode((int)result.ResultCode);

				return TypedResults.Json(result.ErrorMessage, statusCode: (int)result.ResultCode);
		}
	}
}
