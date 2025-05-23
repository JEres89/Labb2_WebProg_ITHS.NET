using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using MinimalAPI.Auth;
using MinimalAPI.DTOs;
using MinimalAPI.DTOs.Requests.Products;
using MinimalAPI.DTOs.Responses.Products;
using MinimalAPI.Services.Products;
using System.Net;

namespace MinimalAPI.Endpoints;

using GetProductsResponseTask =
	Task<Results<
		Ok<ProductsResponse>,
		JsonHttpResult<string>,
		StatusCodeHttpResult>>;

using ProductResponseTask =
	Task<Results<
		Ok<ProductResponse>,
		JsonHttpResult<string>,
		StatusCodeHttpResult>>;

using CreateProductResponseTask =
	Task<Results<
		CreatedAtRoute<ProductResponse>,
		JsonHttpResult<string>,
		StatusCodeHttpResult>>;

using DeleteProductResponseTask =
	Task<Results<
		NoContent,
		JsonHttpResult<string>,
		StatusCodeHttpResult>>;

using GetOrdersResponseTask =
	Task<Results<
		Ok<IEnumerable<ProductOrderResponse>>,
		JsonHttpResult<string>,
		StatusCodeHttpResult>>;

public static class ProductsEndpoints
{
	[HttpGet(Name = "GetProducts")]
	public static async GetProductsResponseTask GetProducts(IProductsActionValidationService validation)
	{
		var result = await validation.GetProductsAsync();
		switch(result.ResultCode)
		{
			case HttpStatusCode.OK:
				return TypedResults.Ok(result.ResultValue!.ToProductsResponse());

			default:
				if(string.IsNullOrEmpty(result.ErrorMessage))
					return TypedResults.StatusCode((int)result.ResultCode);

				return TypedResults.Json(result.ErrorMessage, statusCode: (int)result.ResultCode);
		}
	}

	[HttpPost(Name = "CreateProduct")]
	//[Authorize(Roles = "Admin")]
	public static async CreateProductResponseTask CreateProduct(IProductsActionValidationService validation, WebUser? user, [FromBody] ProductCreateRequest request)
	{
		var product = request.ToProduct();
		var result = await validation.CreateProductAsync(user, product);
		product = result.ResultValue;
		switch(result.ResultCode)
		{
			case HttpStatusCode.OK:
				return TypedResults.CreatedAtRoute(
					value: product!.ToProductResponse(),
					routeName: "GetProduct",
					routeValues: new { id = product!.Id }
				);

			default:
				if(string.IsNullOrEmpty(result.ErrorMessage))
					return TypedResults.StatusCode((int)result.ResultCode);

				return TypedResults.Json(result.ErrorMessage, statusCode: (int)result.ResultCode);
		}
	}

	[HttpGet("{id}", Name = "GetProduct")]
	public static async ProductResponseTask GetProduct(IProductsActionValidationService validation, [FromRoute] int id)
	{
		var result = await validation.GetProductAsync(id);
		switch(result.ResultCode)
		{
			case HttpStatusCode.OK:
				return TypedResults.Ok(result.ResultValue!.ToProductResponse());

			default:
				if(string.IsNullOrEmpty(result.ErrorMessage))
					return TypedResults.StatusCode((int)result.ResultCode);

				return TypedResults.Json(result.ErrorMessage, statusCode: (int)result.ResultCode);
		}
	}

	[HttpPatch("{id}", Name = "UpdateProduct")]
	//[Authorize(Roles = "Admin")]
	public static async ProductResponseTask UpdateProduct(IProductsActionValidationService validation, WebUser? user, [FromRoute] int id, [FromBody] ProductUpdateRequest request)
	{
		var result = await validation.UpdateProductAsync(user, id, request);

		switch(result.ResultCode)
		{
			case HttpStatusCode.OK:
				return TypedResults.Ok(result.ResultValue!.ToProductResponse());

			default:
				if(string.IsNullOrEmpty(result.ErrorMessage))
					return TypedResults.StatusCode((int)result.ResultCode);

				return TypedResults.Json(result.ErrorMessage, statusCode: (int)result.ResultCode);
		}
	}

	[HttpDelete("{id}", Name = "DeleteProduct")]
	//[Authorize(Roles = "Admin")]
	public static async DeleteProductResponseTask DeleteProduct(IProductsActionValidationService validation, WebUser? user, [FromRoute] int id)
	{
		var result = await validation.DeleteProductAsync(user, id);

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

	[HttpGet("{id}/orders", Name = "GetProductOrders")]
	//[Authorize(Roles = "Admin")]
	public static async GetOrdersResponseTask GetOrders(IProductsActionValidationService validation, WebUser? user, [FromRoute] int id)
	{
		var result = await validation.GetProductOrdersAsync(user, id);

		switch(result.ResultCode)
		{
			case HttpStatusCode.OK:
				return TypedResults.Ok(result.ResultValue!.ToProductOrdersResponse());

			default:
				if(string.IsNullOrEmpty(result.ErrorMessage))
					return TypedResults.StatusCode((int)result.ResultCode);

				return TypedResults.Json(result.ErrorMessage, statusCode: (int)result.ResultCode);
		}
	}
}
