using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using MinimalAPI.Auth;
using MinimalAPI.DataModels;
using MinimalAPI.DTOs;
using MinimalAPI.DTOs.Requests.Orders;
using MinimalAPI.DTOs.Responses.Orders;
using MinimalAPI.Services;
using MinimalAPI.Services.Orders;
using System.Net;
using System.Security.Claims;

namespace MinimalAPI.Endpoints;

using GetOrdersResponseTask = 
	Task<Results<
		Ok<OrderCollectionResponse>, 
		JsonHttpResult<string>, 
		StatusCodeHttpResult>>;

using CreateResponseTask =
	Task<Results<
		CreatedAtRoute<OrderResponse>,
		JsonHttpResult<OrderResponse>,
		JsonHttpResult<string>,
		StatusCodeHttpResult>>;

using OrderResponseTask =
	Task<Results<
		Ok<OrderResponse>,
		JsonHttpResult<string>,
		StatusCodeHttpResult>>;

using DeleteResponseTask =
	Task<Results<
		NoContent,
		JsonHttpResult<string>,
		StatusCodeHttpResult>>;
	
using GetProductsResponseTask =
	Task<Results<
		Ok<IEnumerable<OrderProductResponse>>,
		JsonHttpResult<string>,
		StatusCodeHttpResult>>;

using ProductsChangeResponseTask =
	Task<Results<
		Ok<OrderProductsChangeResponse>,
		JsonHttpResult<string>,
		StatusCodeHttpResult>>;

public static class OrdersEndpoints
{
	public static async GetOrdersResponseTask GetOrders(IOrdersActionValidationService validation)
	{
		var result = await validation.GetOrdersAsync();
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

	public static async CreateResponseTask CreateOrder(IOrdersActionValidationService validation, ClaimsPrincipal user, [FromBody] OrderCreateRequest request, HttpContext context)
	{
		var order = request.ToOrder();
		var result = await validation.CreateOrderAsync(user, order, request.Products);
		order = result.ResultValue;
		switch(result.ResultCode)
		{
			case HttpStatusCode.OK:
				return TypedResults.CreatedAtRoute(
					value: order!.ToOrderResponse(),
					routeName: "GetOrder",
					routeValues: new { id = order!.Id }
					);

			case HttpStatusCode.Conflict:
				context.Response.Headers.Append("X-Conflict-Message", result.ErrorMessage);
				return TypedResults.Json(
					data: order!.ToOrderResponse(),
					statusCode: (int)HttpStatusCode.Conflict);

			default:
				if(string.IsNullOrEmpty(result.ErrorMessage))
					return TypedResults.StatusCode((int)result.ResultCode);

				return TypedResults.Json(result.ErrorMessage, statusCode: (int)result.ResultCode);
		}
	}

	public static async OrderResponseTask GetOrder(IOrdersActionValidationService validation, ClaimsPrincipal user, [FromRoute] int id)
	{
		var result = await validation.GetOrderAsync(user, id);
		switch(result.ResultCode)
		{
			case HttpStatusCode.OK:
				return TypedResults.Ok(result.ResultValue!.ToOrderResponse());

			default:
				if(string.IsNullOrEmpty(result.ErrorMessage))
					return TypedResults.StatusCode((int)result.ResultCode);

				return TypedResults.Json(result.ErrorMessage, statusCode: (int)result.ResultCode);
		}
	}

	public static async OrderResponseTask UpdateOrder(IOrdersActionValidationService validation, ClaimsPrincipal user, [FromRoute] int id, [FromBody] OrderUpdateRequest request)
	{
		var result = await validation.UpdateOrderAsync(user, id, request.Status);

		switch(result.ResultCode)
		{
			case HttpStatusCode.OK:
				return TypedResults.Ok(result.ResultValue!.ToOrderResponse());

			default:
				if(string.IsNullOrEmpty(result.ErrorMessage))
					return TypedResults.StatusCode((int)result.ResultCode);

				return TypedResults.Json(result.ErrorMessage, statusCode: (int)result.ResultCode);
		}
	}

	public static async DeleteResponseTask DeleteOrder(IOrdersActionValidationService validation, ClaimsPrincipal user, [FromRoute] int id)
	{
		var result = await validation.DeleteOrderAsync(user, id);

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

	public static async GetProductsResponseTask GetProducts(IOrdersActionValidationService validation, ClaimsPrincipal user, [FromRoute] int id)
	{
		var result = await validation.GetProductsAsync(user, id);

		switch(result.ResultCode)
		{
			case HttpStatusCode.OK:
				return TypedResults.Ok(result.ResultValue!.ToOrderProductsResponse());

			default:
				if(string.IsNullOrEmpty(result.ErrorMessage))
					return TypedResults.StatusCode((int)result.ResultCode);

				return TypedResults.Json(result.ErrorMessage, statusCode: (int)result.ResultCode);
		}
	}

	public static async ProductsChangeResponseTask SetProducts(IOrdersActionValidationService validation, ClaimsPrincipal user, [FromRoute] int id, [FromBody] OrderProductsChangeRequest request)
		=> await UpdateProducts(validation, user, id, request, false);

	public static async ProductsChangeResponseTask ReplaceProducts(IOrdersActionValidationService validation, ClaimsPrincipal user, [FromRoute] int id, [FromBody] OrderProductsChangeRequest request)
		=> await UpdateProducts(validation, user, id, request, true);

	private static async ProductsChangeResponseTask UpdateProducts(IOrdersActionValidationService validation, ClaimsPrincipal user, int id, OrderProductsChangeRequest request, bool replace)
	{
		var result = replace
			? await validation.UpdateProductsAsync(user, id, request.Products, true)
			: await validation.UpdateProductsAsync(user, id, request.Products);
		switch(result.ResultCode)
		{
			case HttpStatusCode.OK:
				return TypedResults.Ok(result.ResultValue!.ToOrderProductsChangeResponse());

			default:
				if(string.IsNullOrEmpty(result.ErrorMessage))
					return TypedResults.StatusCode((int)result.ResultCode);

				return TypedResults.Json(result.ErrorMessage, statusCode: (int)result.ResultCode);
		}
	}
}
