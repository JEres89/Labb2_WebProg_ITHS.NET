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
using static MinimalAPI.Services.ValidationResultCode;

namespace MinimalAPI.Endpoints;

public static class OrdersEndpoints
{
	[HttpGet(Name = "GetOrders")]
	//[Authorize(Roles = "Admin")]
	public static async Task<Results<Ok<OrdersResponse>, UnauthorizedHttpResult, NoContent, BadRequest<string?>, StatusCodeHttpResult>> GetOrders(IOrdersActionValidationService validation, WebUser? user)
	{
		var result = await validation.GetOrdersAsync(user);
		switch(result.ResultCode)
		{
			case Success:
				if(result.ResultValue == null)
				{
					return TypedResults.NoContent();
				}
				return TypedResults.Ok(result.ResultValue!.ToOrdersResponse());

			case Unauthorized:
				return TypedResults.Unauthorized();

			case Failed:
				return TypedResults.BadRequest<string?>(result.ErrorMessage);

			default:
				return TypedResults.StatusCode(StatusCodes.Status500InternalServerError);
		}
	}

	[HttpPost(Name = "CreateOrder")]
	//[Authorize]
	public static async Task<Results<CreatedAtRoute<OrderResponse>, UnauthorizedHttpResult, BadRequest<string?>, StatusCodeHttpResult>> CreateOrder(IOrdersActionValidationService validation, WebUser? user, [FromBody] OrderCreateRequest request)
	{
		var order = request.ToOrder();
		var result = await validation.CreateOrderAsync(user, order, request.Products);
		order = result.ResultValue;
		switch(result.ResultCode)
		{
			case Success:
				return TypedResults.CreatedAtRoute(
					value: order!.ToOrderResponse(),
					routeName: "GetOrder",
					routeValues: new { id = order!.Id }
					);

			case Unauthorized:
				return TypedResults.Unauthorized();

			case Failed:
				return TypedResults.BadRequest<string?>(result.ErrorMessage);

			default:
				return TypedResults.StatusCode(StatusCodes.Status500InternalServerError);
		}
	}

	[HttpGet("{id}", Name = "GetOrder")]
	//[Authorize]
	public static async Task<Results<Ok<OrderResponse>, UnauthorizedHttpResult, NotFound, BadRequest<string?>, StatusCodeHttpResult>> GetOrder(IOrdersActionValidationService validation, WebUser? user, [FromRoute] int id)
	{
		var result = await validation.GetOrderAsync(user, id);
		switch(result.ResultCode)
		{
			case Success:
				return TypedResults.Ok(result.ResultValue!.ToOrderResponse());

			case Unauthorized:
				return TypedResults.Unauthorized();

			case ValidationResultCode.NotFound:
				return TypedResults.NotFound();

			default:
				return TypedResults.StatusCode(StatusCodes.Status500InternalServerError);
		}
	}

	[HttpPatch("{id}", Name = "UpdateOrder")]
	//[Authorize]
	public static async Task<Results<Ok<OrderResponse>, UnauthorizedHttpResult, NotFound, BadRequest<string?>, StatusCodeHttpResult>> UpdateOrder(IOrdersActionValidationService validation, WebUser? user, [FromRoute] int id, [FromBody] OrderUpdateRequest request)
	{
		var result = await validation.UpdateOrderAsync(user, id, request.Status);

		switch(result.ResultCode)
		{
			case Success:
				return TypedResults.Ok(result.ResultValue!.ToOrderResponse());

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

	[HttpDelete("{id}", Name = "DeleteOrder")]
	//[Authorize]
	public static async Task<Results<NoContent, UnauthorizedHttpResult, NotFound, BadRequest, StatusCodeHttpResult>> DeleteOrder(IOrdersActionValidationService validation, WebUser? user, [FromRoute] int id)
	{
		switch(await validation.DeleteOrderAsync(user, id))
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

	[HttpGet("{id}/products", Name = "GetOrderProducts")]
	//[Authorize]
	public static async Task<Results<NoContent, Ok<IEnumerable<OrderProductResponse>>, UnauthorizedHttpResult, NotFound, StatusCodeHttpResult>> GetProducts(IOrdersActionValidationService validation, WebUser? user, [FromRoute] int id)
	{
		var result = await validation.GetProductsAsync(user, id);

		switch(result.ResultCode)
		{
			case Success:
				if(result.ResultValue == null)
				{
					return TypedResults.NoContent();
				}
				return TypedResults.Ok(result.ResultValue.ToOrderProductsResponse());

			case Unauthorized:
				return TypedResults.Unauthorized();

			case ValidationResultCode.NotFound:
				return TypedResults.NotFound();

			default:
				return TypedResults.StatusCode(StatusCodes.Status500InternalServerError);
		}
	}

	[HttpPatch("{id}/products", Name = "SetProducts")]
	//[Authorize]
	public static async Task<Results<NoContent, Ok<OrderProductsChangeResponse>, UnauthorizedHttpResult, NotFound, BadRequest<string?>, StatusCodeHttpResult>> SetProducts(IOrdersActionValidationService validation, WebUser? user, [FromRoute] int id, [FromBody] OrderProductsChangeRequest request)
	{
		var result = await validation.UpdateProductsAsync(user, id, request.Products);
		switch(result.ResultCode)
		{
			case Success:
				if(result.ResultValue == null)
				{
					return TypedResults.NoContent();
				}
				return TypedResults.Ok(result.ResultValue.ToOrderProductsChangeResponse());

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

	[HttpPut("{id}/products", Name = "ReplaceProducts")]
	//[Authorize]
	public static async Task<Results<NoContent, Ok<OrderProductsChangeResponse>, UnauthorizedHttpResult, NotFound, BadRequest<string?>, StatusCodeHttpResult>> ReplaceProducts(IOrdersActionValidationService validation, WebUser? user, [FromRoute] int id, [FromBody] OrderProductsChangeRequest request)
	{
		var result = await validation.SetProductsAsync(user, id, request.Products);
		switch(result.ResultCode)
		{
			case Success:
				if(result.ResultValue == null)
				{
					return TypedResults.NoContent();
				}
				return TypedResults.Ok(result.ResultValue.ToOrderProductsChangeResponse());

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
