using Microsoft.Data.SqlClient;
using MinimalAPI.Auth;
using MinimalAPI.DataModels;
using System.ComponentModel.DataAnnotations;
using System.Net;
using static System.Net.HttpStatusCode;

namespace MinimalAPI.Services.Products;

public class ProductsActionValidationService : IProductsActionValidationService
{
	private readonly IUnitOfWork _worker;
	public ProductsActionValidationService(IUnitOfWork worker)
	{
		_worker = worker;
	}
	
	public async Task<ValidationResult<IEnumerable<Product>>> GetProductsAsync()
	{
		var canWork = await _worker.BeginWork<IEnumerable<Product>>(false);
		if(canWork.ResultCode != Continue)
			return canWork;

		var repo = _worker.Products;
		var products = await repo.GetProductsAsync();

		if(products == null)
			return new ValidationResult<IEnumerable<Product>> {
				ResultCode = InternalServerError,	
				ErrorMessage = "Could not retreive Products" 
			};

		else if(products.Count() == 0)
			return new ValidationResult<IEnumerable<Product>> { 
				ResultCode = NoContent 
			};

		else
			return new ValidationResult<IEnumerable<Product>> {
				ResultCode = OK,
				ResultValue = products 
			};
	}

	public async Task<ValidationResult<Product>> CreateProductAsync(Product product)
	{
		var canWork = await _worker.BeginWork<Product>(true);
		if(canWork.ResultCode != Continue)
			return canWork;

		try
		{
			var newProduct = await _worker.Products.CreateProductAsync(product);
			var changes = await _worker.SaveChangesAsync<Product>();

			if(changes.ResultCode.IsSuccessCode())
			{
				changes.ResultValue = newProduct;
				return changes;
			}

			else
				return changes;
		}
		catch(SqlException se)
		{
			if(ErrorHelper.DuplicateSqlErrors.Contains(se.Number)) // Unique constraint errors
				return await ErrorHelper.RollbackOnSqlServerDuplicateError<Product>(_worker, se);
			else
				return await ErrorHelper.RollbackOnSqlServerError<Product>(_worker, se, "creating the Product");
		}
		catch(Exception e)
		{
			return await ErrorHelper.RollbackOnServerError<Product>(_worker, e);
		}
	}

	public async Task<ValidationResult<Product>> GetProductAsync(int id)
	{
		var canWork = await _worker.BeginWork<Product>(false);
		if(canWork.ResultCode != Continue)
			return canWork;

		var repo = _worker.Products;
		var product = await repo.GetProductAsync(id);

		return new ValidationResult<Product> { 
			ResultCode = product == null ? NotFound : OK, 
			ResultValue = product 
		};
	}

	public async Task<ValidationResult<Product>> UpdateProductAsync(int id, Dictionary<string, string> updates)
	{
		if(updates == null || updates.Count == 0)
			return new ValidationResult<Product> { 
				ResultCode = BadRequest, 
				ErrorMessage = "No properties were provided" 
			};

		var canWork = await _worker.BeginWork<Product>(true);
		if(canWork.ResultCode != Continue)
			return canWork;

		try
		{
			var updatedProduct = await _worker.Products.UpdateProductAsync(id, updates);

			if(updatedProduct == null)
				return new ValidationResult<Product> {
					ResultCode = NotFound,
					ErrorMessage = $"Product with id {id} could not be found." 
				};

			var changes = await _worker.SaveChangesAsync<Product>();

			if(changes.ResultCode.IsSuccessCode())
			{
				changes.ResultValue = updatedProduct;
				return changes;
			}

			else
				return changes;
		}
		catch(SqlException se)
		{
			return await ErrorHelper.RollbackOnSqlServerError<Product>(_worker, se, "updating the Product");
		}
		catch(Exception e)
		{
			return await ErrorHelper.RollbackOnServerError<Product>(_worker, e);
		}
	}

	public async Task<ValidationResult<int>> DeleteProductAsync(int id)
	{
		var canWork = await _worker.BeginWork<int>(true);
		if(canWork.ResultCode != Continue)
			return canWork;

		try
		{
			var success = await _worker.Products.DeleteProductAsync(id);

			if(!success)
				return new ValidationResult<int> { 
					ResultCode = NotFound,
					ErrorMessage = $"Product with id {id} could not be found." 
				};

			return await _worker.SaveChangesAsync<int>();
		}
		catch(SqlException se)
		{
			return await ErrorHelper.RollbackOnSqlServerError<int>(_worker, se, "deleting the Product");
		}
		catch(Exception e)
		{
			return await ErrorHelper.RollbackOnServerError<int>(_worker, e);
		}
	}

	public async Task<ValidationResult<IEnumerable<OrderProduct>>> GetProductOrdersAsync(int id)
	{
		var canWork = await _worker.BeginWork<IEnumerable<OrderProduct>>(false);
		if(canWork.ResultCode != Continue)
			return canWork;

		var orders = await _worker.Orders.FindOrdersForProductAsync(null, id);

		if(orders == null)
			return new ValidationResult<IEnumerable<OrderProduct>> { 
				ResultCode = NotFound,
				ErrorMessage = $"Product with id {id} could not be found." 
			}; 

		else if(orders.Count() == 0)
			return new ValidationResult<IEnumerable<OrderProduct>> { 
				ResultCode = NoContent 
			};

		else
			return new ValidationResult<IEnumerable<OrderProduct>> {
				ResultCode = OK,
				ResultValue = orders 
			};
	}
}
