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
		Ok<ProductGetAllResponse>,
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

	public static async CreateProductResponseTask CreateProduct(IProductsActionValidationService validation, [FromBody] ProductCreateRequest request)
	{
		var product = request.ToProduct();
		var result = await validation.CreateProductAsync(product);
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

	public static async ProductResponseTask UpdateProduct(IProductsActionValidationService validation, [FromRoute] int id, [FromBody] ProductUpdateRequest request)
	{
		var result = await validation.UpdateProductAsync(id, request);

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

	public static async DeleteProductResponseTask DeleteProduct(IProductsActionValidationService validation, [FromRoute] int id)
	{
		var result = await validation.DeleteProductAsync(id);

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

	public static async GetOrdersResponseTask GetOrders(IProductsActionValidationService validation, [FromRoute] int id)
	{
		var result = await validation.GetProductOrdersAsync(id);

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
