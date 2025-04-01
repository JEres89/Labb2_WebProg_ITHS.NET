using MinimalAPI.Auth;
using MinimalAPI.DataModels;

namespace MinimalAPI.Services.Products;

public interface IProductsRepository
{
	Task<IEnumerable<Product>> GetProductsAsync();
	Task<Product> CreateProductAsync(Product product);
	Task<Product> GetProductAsync(int id);
	Task<Product> UpdateProductAsync(int id, Product product);
	Task<bool> DeleteProductAsync(int id);
}

public interface IProductsActionValidationService
{
	Task<ValidationResult<IEnumerable<Product>>> GetProductsAsync();
	Task<ValidationResult<Product>> GetProductAsync(int id);
	Task<ValidationResult<Product>> CreateProductAsync(WebUser? user, Product product);
	Task<ValidationResult<Product>> UpdateProductAsync(WebUser? user, int id, Dictionary<string, string> product);
	Task<ValidationResultCode> DeleteProductAsync(WebUser? user, int id);
	Task<ValidationResult<IEnumerable<OrderProduct>>> GetProductOrdersAsync(WebUser? user, int id);
}
