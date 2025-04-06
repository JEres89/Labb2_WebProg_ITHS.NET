using MinimalAPI.Auth;
using MinimalAPI.DataModels;
using static MinimalAPI.Services.ValidationResultCode;

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
		var repo = _worker.Products;
		var products = await repo.GetProductsAsync();

		if(products == null)
		{
			return new ValidationResult<IEnumerable<Product>>
			{
				ResultCode = Failed,
				ErrorMessage = "Could not retreive Products"
			};
		}
		else if(!products.Any())
			return new ValidationResult<IEnumerable<Product>> { ResultCode = Success };

		else
		{
			return new ValidationResult<IEnumerable<Product>>
			{
				ResultCode = Success,
				ResultValue = products
			};
		}
	}

	public async Task<ValidationResult<Product>> CreateProductAsync(WebUser? user, Product product)
	{
		if(user == null || user.Role != Role.Admin)
			return new ValidationResult<Product> { ResultCode = Unauthorized };

		var repo = _worker.Products;
		var newProduct = await repo.CreateProductAsync(product);
		var changes = await _worker.SaveChangesAsync();

		return new ValidationResult<Product>
		{
			ResultCode = changes > 0 ? Success : Failed,
			ResultValue = newProduct
		};
	}

	public async Task<ValidationResult<Product>> GetProductAsync(int id)
	{
		var repo = _worker.Products;
		var product = await repo.GetProductAsync(id);

		return new ValidationResult<Product>
		{
			ResultCode = product == null ? NotFound : Success,
			ResultValue = product
		};
	}

	public async Task<ValidationResult<Product>> UpdateProductAsync(WebUser? user, int id, Dictionary<string, string> updates)
	{
		if(user == null || user.Role != Role.Admin)
			return new ValidationResult<Product> { ResultCode = Unauthorized };

		var repo = _worker.Products;
		var product = await repo.GetProductAsync(id);

		if(product == null)
			return new ValidationResult<Product> { ResultCode = NotFound };

		var updatedProduct = await repo.UpdateProductAsync(id, updates);

		return new ValidationResult<Product>
		{
			ResultCode = updatedProduct != null ? Success : Failed,
			ResultValue = updatedProduct,
			ErrorMessage = updatedProduct != null ? null : "No changes were made"
		};
	}

	public async Task<ValidationResultCode> DeleteProductAsync(WebUser? user, int id)
	{
		if(user == null || user.Role != Role.Admin)
			return Unauthorized;

		var repo = _worker.Products;
		var success = await repo.DeleteProductAsync(id);
		var changes = await _worker.SaveChangesAsync();

		return changes > 0 ? Success : Failed;
	}

	public async Task<ValidationResult<IEnumerable<OrderProduct>>> GetProductOrdersAsync(WebUser? user, int id)
	{
		if(user == null || user.Role != Role.Admin)
			return new ValidationResult<IEnumerable<OrderProduct>> { ResultCode = Unauthorized };

		var repo = _worker.Orders;
		var orders = await repo.FindOrdersForProductAsync(null, id);

		if(orders == null)
			return new ValidationResult<IEnumerable<OrderProduct>> { ResultCode = NotFound };

		else if(!orders.Any())
			return new ValidationResult<IEnumerable<OrderProduct>> { ResultCode = Success };

		else
		{
			return new ValidationResult<IEnumerable<OrderProduct>>
			{
				ResultCode = Success,
				ResultValue = orders
			};
		}
	}
}
