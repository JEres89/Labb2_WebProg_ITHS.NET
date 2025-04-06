using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using MinimalAPI.Auth;
using MinimalAPI.DTOs.Requests.Products;
using MinimalAPI.DTOs.Responses.Products;
using MinimalAPI.DTOs.Responses.Orders;
using MinimalAPI.Mappers;
using MinimalAPI.Services;
using MinimalAPI.Services.Products;
using static MinimalAPI.Services.ValidationResultCode;

namespace MinimalAPI.Endpoints;

public static class ProductsEndpoints
{
	[HttpGet(Name = "GetProducts")]
	public static async Task<Results<Ok<ProductsResponse>, NoContent, BadRequest<string?>, StatusCodeHttpResult>> GetProducts(IProductsActionValidationService validation)
	{
		var result = await validation.GetProductsAsync();
		switch(result.ResultCode)
		{
			case Success:
				if(result.ResultValue == null)
				{
					return TypedResults.NoContent();
				}
				return TypedResults.Ok(result.ResultValue!.ToProductsResponse());

			case Failed:
				return TypedResults.BadRequest<string?>(result.ErrorMessage);

			default:
				return TypedResults.StatusCode(StatusCodes.Status500InternalServerError);
		}
	}

	[HttpPost(Name = "CreateProduct")]
	//[Authorize(Roles = "Admin")]
	public static async Task<Results<CreatedAtRoute<ProductResponse>, UnauthorizedHttpResult, BadRequest<string?>, StatusCodeHttpResult>> CreateProduct(IProductsActionValidationService validation, WebUser? user, [FromBody] ProductCreateRequest request)
	{
		var product = request.ToProduct();
		var result = await validation.CreateProductAsync(user, product);
		product = result.ResultValue;
		switch(result.ResultCode)
		{
			case Success:
				return TypedResults.CreatedAtRoute(
					value: product!.ToProductResponse(),
					routeName: "GetProduct",
					routeValues: new { id = product!.Id }
					);

			case Unauthorized:
				return TypedResults.Unauthorized();

			case Failed:
				return TypedResults.BadRequest<string?>(result.ErrorMessage);

			default:
				return TypedResults.StatusCode(StatusCodes.Status500InternalServerError);
		}
	}

	[HttpGet("{id}", Name = "GetProduct")]
	public static async Task<Results<Ok<ProductResponse>, NotFound, BadRequest<string?>, StatusCodeHttpResult>> GetProduct(IProductsActionValidationService validation, [FromRoute] int id)
	{
		var result = await validation.GetProductAsync(id);
		switch(result.ResultCode)
		{
			case Success:
				return TypedResults.Ok(result.ResultValue!.ToProductResponse());

			case ValidationResultCode.NotFound:
				return TypedResults.NotFound();

			default:
				return TypedResults.StatusCode(StatusCodes.Status500InternalServerError);
		}
	}

	[HttpPatch("{id}", Name = "UpdateProduct")]
	//[Authorize(Roles = "Admin")]
	public static async Task<Results<Ok<ProductResponse>, UnauthorizedHttpResult, NotFound, BadRequest<string?>, StatusCodeHttpResult>> UpdateProduct(IProductsActionValidationService validation, WebUser? user, [FromRoute] int id, [FromBody] ProductUpdateRequest request)
	{
		var result = await validation.UpdateProductAsync(user, id, request);

		switch(result.ResultCode)
		{
			case Success:
				return TypedResults.Ok(result.ResultValue!.ToProductResponse());

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

	[HttpDelete("{id}", Name = "DeleteProduct")]
	//[Authorize(Roles = "Admin")]
	public static async Task<Results<NoContent, UnauthorizedHttpResult, NotFound, BadRequest, StatusCodeHttpResult>> DeleteProduct(IProductsActionValidationService validation, WebUser? user, [FromRoute] int id)
	{
		switch(await validation.DeleteProductAsync(user, id))
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

	[HttpGet("{id}/orders", Name = "GetProductOrders")]
	//[Authorize(Roles = "Admin")]
	public static async Task<Results<NoContent, Ok<IEnumerable<ProductOrderResponse>>, UnauthorizedHttpResult, NotFound, /*BadRequest<string?>,*/ StatusCodeHttpResult>> GetOrders(IProductsActionValidationService validation, WebUser? user, [FromRoute] int id)
	{
		var result = await validation.GetProductOrdersAsync(user, id);

		switch(result.ResultCode)
		{
			case Success:
				return result.ResultValue == null 
					? TypedResults.NoContent() 
					: TypedResults.Ok(result.ResultValue.ToProductOrdersResponse());

			case Unauthorized:
				return TypedResults.Unauthorized();

			case ValidationResultCode.NotFound:
				return TypedResults.NotFound();

			//case Failed:
			//	return TypedResults.BadRequest<string?>(result.ErrorMessage);

			default:
				return TypedResults.StatusCode(StatusCodes.Status500InternalServerError);
		}
	}
}
