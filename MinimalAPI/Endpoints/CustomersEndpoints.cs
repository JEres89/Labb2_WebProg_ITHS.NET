using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using MinimalAPI.Auth;
using MinimalAPI.DTOs;
using MinimalAPI.DTOs.Requests.Customers;
using MinimalAPI.DTOs.Responses.Customers;
using MinimalAPI.DTOs.Responses.Orders;
using MinimalAPI.Services.Customers;
using System.Net;
using System.Security.Claims;

namespace MinimalAPI.Endpoints;

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
using GetCustomersResponseTask =
	Task<Results<
		Ok<CustomerCollectionResponse>,
		JsonHttpResult<string>,
		StatusCodeHttpResult>>;
using GetOrdersResponseTask =
	Task<Results<
		Ok<OrderCollectionResponse>,
		JsonHttpResult<string>,
		StatusCodeHttpResult>>;

public static class CustomersEndpoints
{
	// TODO: Add these to all methods since the TypedResult-swagger interactions are broken.
	[ProducesResponseType((int)HttpStatusCode.NoContent)]
	[ProducesResponseType((int)HttpStatusCode.Unauthorized)]
	[ProducesResponseType(typeof(string), (int)HttpStatusCode.ServiceUnavailable)]
	[ProducesResponseType(typeof(string), (int)HttpStatusCode.InternalServerError)]
	public static async GetCustomersResponseTask GetCustomers(ICustomersActionValidationService validation)
	{
		var result = await validation.GetCustomersAsync();
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

	public static async CreateCustomerResponseTask CreateCustomer(ICustomersActionValidationService validation, ClaimsPrincipal user, [FromBody] CustomerCreateRequest request)
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

	public static async CustomerResponseTask GetCustomer(ICustomersActionValidationService validation, ClaimsPrincipal user, [FromRoute] int id)
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

	public static async CustomerResponseTask UpdateCustomer(ICustomersActionValidationService validation, ClaimsPrincipal user, [FromRoute] int id, [FromBody] CustomerUpdateRequest request)
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

	public static async DeleteCustomerResponseTask DeleteCustomer(ICustomersActionValidationService validation, ClaimsPrincipal user, [FromRoute] int id)
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

	public static async GetOrdersResponseTask GetOrders(ICustomersActionValidationService validation, ClaimsPrincipal user, [FromRoute] int id)
	{
		var result = await validation.GetOrdersAsync(user, id);

		switch(result.ResultCode)
		{
			case HttpStatusCode.OK:
				return TypedResults.Ok(result.ResultValue!.ToOrderCollectionResponse());

			default:
				if(string.IsNullOrEmpty(result.ErrorMessage))
					return TypedResults.StatusCode((int)result.ResultCode);

				return TypedResults.Json(result.ErrorMessage, statusCode: (int)result.ResultCode);
		}
	}
}
